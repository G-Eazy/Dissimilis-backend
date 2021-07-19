using System.Linq;
using System.Threading.Tasks;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Interfaces;
using System;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Dissimilis.DbContext;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongExtension
    {
        /// <summary>
        /// Get max bar positions for a song
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        public static int GetMaxBarPosition(this Song song)
        {
            var eightParts = 8 / song.Denominator;

            var amountOfWhole = (double)song.Numerator / song.Denominator;

            return (int)((eightParts * song.Denominator) * amountOfWhole) - 1;
        }

        /// <summary>
        /// Ensures all voices has the same amount of bars
        /// </summary>
        /// <param name="song"></param>
        public static void SyncBarCountToMaxInAllVoices(this Song song)
        {

            var maxBarCount = song.Voices
                .Select(v => v.SongBars.Count)
                .OrderByDescending(v => v)
                .FirstOrDefault();
            if (maxBarCount == 0)
            {
                maxBarCount = 1;
            }

            foreach (var songVoice in song.Voices)
            {
                var highestBarNumber = songVoice.SongBars.Select(sb => sb.Position)
                    .OrderByDescending(v => v)
                    .FirstOrDefault();

                highestBarNumber++;
                while (songVoice.SongBars.Count < maxBarCount)
                {
                    songVoice.SongBars = songVoice.SongBars.Concat(new[] { new SongBar(highestBarNumber++) }).ToArray();
                }

                songVoice.SortBars();
            }
        }

        /// <summary>
        /// Sets the song and all voices to same Updated datetime and user
        /// </summary>
        /// <param name="song"></param>
        /// <param name="userId"></param>
        public static void SetUpdatedOverAll(this Song song, int userId)
        {
            foreach (var songVoice in song.Voices)
            {
                Console.WriteLine(songVoice.VoiceName);
                songVoice.SetSongVoiceUpdated(userId);
            }
            song.SetUpdated(userId);
        }

        /// <summary>
        /// Copies (length of) bars from one position to another position to in the song
        /// </summary>
        public static void CopyBars(this Song song, int fromPosition, int copyLength, int toPosition)
        {
            SyncBarCountToMaxInAllVoices(song);

            var firstVoice = song.Voices.FirstOrDefault();
            if (firstVoice == null || firstVoice.SongBars.Count() < fromPosition)
            {
                throw new NotFoundException("Voice or bar not found");
            }

            foreach (var voice in song.Voices)
            {
                foreach (var bar in voice.SongBars)
                {
                    if (bar.Position >= toPosition)
                    {
                        bar.Position += copyLength;
                    }
                }

                var barsToInsert = voice.SongBars.OrderBy(b => b.Position)
                    .Skip(fromPosition - 1)
                    .Take(copyLength)
                    .Select(b => b.Clone())
                    .ToArray();
                var startIndex = toPosition;
                foreach (var insertBar in barsToInsert)
                {
                    insertBar.Position = startIndex++;
                }

                voice.SongBars = voice.SongBars.Concat(barsToInsert).ToArray();
            }

        }

        /// <summary>
        /// Copies (length of) bars from one position to another position to in the song
        /// </summary>
        public static void MoveBars(this Song song, int fromPosition, int moveLength, int toPosition)
        {
            SyncBarCountToMaxInAllVoices(song);

            var firstVoice = song.Voices.FirstOrDefault();
            if (firstVoice == null || firstVoice.SongBars.Count() < fromPosition)
            {
                throw new NotFoundException("Voice or bar not found");
            }

            foreach (var voice in song.Voices)
            {
                foreach (var bar in voice.SongBars)
                {
                    if (bar.Position > toPosition)
                    {
                        bar.Position += moveLength;
                    }
                    else
                    {
                        bar.Position -= moveLength;
                    }
                }

                var barsToMove = voice.SongBars.OrderBy(b => b.Position)
                    .Skip(fromPosition - 1)
                    .Take(moveLength)
                    .ToArray();
                var startIndex = toPosition;
                foreach (var insertBar in barsToMove)
                {
                    insertBar.Position = startIndex++;
                }
            }

            SyncBarCountToMaxInAllVoices(song);
        }

        /// <summary>
        /// Deletes bars 
        /// </summary>
        public static void DeleteBars(this Song song, int fromPosition, int deleteLength)
        {
            SyncBarCountToMaxInAllVoices(song);

            var firstVoice = song.Voices.FirstOrDefault();
            var firstVoiceLength = firstVoice.SongBars.Count();
            if (firstVoice == null || firstVoiceLength < fromPosition)
            {
                throw new NotFoundException("Voice or bar not found");
            }

            foreach (var voice in song.Voices.ToList())
            {
                foreach (var bar in voice.SongBars.ToList())
                {
                    if (fromPosition <= bar.Position && bar.Position < deleteLength + fromPosition)
                    {
                        voice.SongBars.Remove(bar);
                    }
                    if (bar.Position >= deleteLength + fromPosition)
                    {
                        bar.Position -= deleteLength;
                    }
                }
            }
            
            if(firstVoiceLength == deleteLength)
            { 
                foreach(var voice in song.Voices)
                {
                    voice.SongBars.Add(new SongBar());
                }
            }

            SyncBarCountToMaxInAllVoices(song);
        }

        /// <summary>
        /// Removes a bar from a given position from all voices
        /// </summary>
        /// <param name="song"></param>
        /// <param name="position"></param>
        public static void RemoveSongBarFromAllVoices(this Song song, int position)
        {
            foreach (var songVoice in song.Voices)
            {
                var barToRemove = songVoice.SongBars.FirstOrDefault(sb => sb.Position == position) ?? songVoice.SongBars.LastOrDefault();

                songVoice.SongBars.Remove(barToRemove);
                songVoice.SortBars();
            }
        }

        /// <summary>
        /// Ensure all other voices in the song has the same setup as the masterVoice
        /// </summary>
        /// <param name="song"></param>
        /// <param name="masterVoice"></param>
        public static void SyncVoicesFrom(this Song song, SongVoice masterVoice)
        {
            var otherVoices = song.Voices.Where(v => v.Id != masterVoice.Id).ToArray();
            if (!otherVoices.Any())
            {
                return;
            }

            SyncBarCountToMaxInAllVoices(song);

            var barLength = masterVoice.SongBars.Count;
            for (var i = 0; i < barLength; i++)
            {
                foreach (var otherVoice in otherVoices)
                {
                    var masterBar = masterVoice.SongBars.ToArray()[i];
                    var slaveBar = otherVoice.SongBars.ToArray()[i];

                    slaveBar.House = masterBar.House;
                    slaveBar.RepAfter = masterBar.RepAfter;
                    slaveBar.RepBefore = masterBar.RepBefore;
                }
            }

        }

        public static Song Clone(this Song song, string title = null)
        {
            return new Song()
            {
                Title = title ?? song.Title,
                Denominator = song.Denominator,
                Numerator = song.Numerator,
                ArrangerId = song.ArrangerId,
                Speed = song.Speed,
                DegreeOfDifficulty = song.DegreeOfDifficulty,
                SongNotes = song.SongNotes,
                Voices = song.Voices.Select(v => v.Clone(v.VoiceName)).ToArray()
            };
        }

        public static Song Transpose(this Song song, int transpose = 0)
        {
            song.Voices = song.Voices.Select(v => v.Transpose(transpose)).ToArray();

            return song;
        }

        public static void RemoveElementsFromOldSong(this Song song, SongVoiceDto[] deserialisedVoices)
        {
            var voices = song.Voices;
            foreach(var voice in voices)
            {
                var foundVoice = deserialisedVoices.SingleOrDefault(v => v.SongVoiceId == voice.Id);
                if (foundVoice == null)
                {
                    song.Voices.Remove(voice);
                    continue;
                }
                BarDto[] deserialisedBars = foundVoice.Bars;
                foreach(var bar in voice.SongBars)
                {
                    var foundBar = deserialisedBars.SingleOrDefault(b => b.BarId == bar.Id);
                    if (foundBar == null)
                    {
                        voice.SongBars.Remove(bar);
                        continue;
                    }
                    NoteDto[] deserialisedNotes = foundBar.Chords;
                    foreach(var noteDto in deserialisedNotes)
                    {
                        bool emptyNote = noteDto.Notes[0] == "Z";
                        if (emptyNote)
                        {
                            for (int i = noteDto.Position; i < noteDto.Position + noteDto.Length; i++)
                            {
                                var noteToBeRemoved = bar.Notes.SingleOrDefault(n => n.Position == i);
                                if (noteToBeRemoved != null)
                                    bar.Notes.Remove(noteToBeRemoved);
                            }
                        }
                    }
                    foreach (var note in bar.Notes)
                    {
                        var foundNote = deserialisedNotes.SingleOrDefault(n => n.ChordId == note.Id);
                        if (foundNote == null)
                            bar.Notes.Remove(note);
                    }
                }
            }
        }

        public static void AddSnapshotValues(this Song song, SongByIdDto deserialisedSong, User updatedBy)
        {
            song.Title = deserialisedSong.Title;
            song.UpdatedBy = updatedBy;
            song.UpdatedOn = DateTimeOffset.Now;
            foreach(var voiceDto in deserialisedSong.Voices)
            {
                SongVoice voice = SongVoiceDto.ConvertToSongVoice(voiceDto, DateTimeOffset.Now, updatedBy.Id, song);
                foreach(var barDto in voiceDto.Bars)
                {
                    SongBar bar = BarDto.ConvertToSongBar(barDto, voice);
                    foreach(var noteDto in barDto.Chords)
                    {
                        if (noteDto.Notes[0] != "Z")
                            bar.Notes.Add(NoteDto.ConvertToSongNote(noteDto, bar));
                    }
                    voice.SongBars.Add(bar);
                }
                song.Voices.Add(voice);
            }
        }

        public static (Song, List<SongNote>) GetUndoneSong(this Song song)
        {
            if (song.Snapshots.Count == 0)
                throw new NotFoundException("No more snapshots to pop...");

            SongSnapshot snapshot = song.PopSnapshot(true);
            Console.WriteLine(snapshot.SongObjectJSON);
            SongByIdDto deserialisedSong = Newtonsoft.Json.JsonConvert.DeserializeObject<SongByIdDto>(snapshot.SongObjectJSON);
            
            Song undoneSong = new Song() { };
            List<SongNote> removeNotes;
            undoneSong.Title = deserialisedSong.Title;
            undoneSong.UpdatedBy = snapshot.CreatedBy;
            undoneSong.UpdatedOn = DateTimeOffset.Now;
            (undoneSong.Voices, removeNotes) = SongVoiceExtension.GetSongVoicesFromDto(song, snapshot, deserialisedSong.Voices);
            
            /*JObject deserialisedSong = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(snapshot.SongObjectJSON);
            JArray voiceJSONs = deserialisedSong["Voices"].Value<JArray>();*/

            // Construct new song from snapshot
            /*song.Title = deserialisedSong.Title;
            song.UpdatedBy = snapshot.CreatedBy;
            song.UpdatedOn = DateTimeOffset.Now;
            song.Voices = SongVoiceExtension.GetSongVoicesFromDto(song, snapshot, deserialisedSong.Voices);

            Console.WriteLine("voices left");
            foreach (var voice in song.Voices) Console.WriteLine("voice");
            Console.WriteLine("/////////////////////////////");*/

            return (undoneSong, removeNotes);
        }

        /// <summary>
        /// This method should be called before any action that updates a song, except when the song itself is created.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="user"></param>
        public static void PerformSnapshot(this Song song, User user)
        {
            string JSONsnapshot = Newtonsoft.Json.JsonConvert.SerializeObject(new SongByIdDto(song));
            Console.WriteLine(JSONsnapshot);

            SongSnapshot snapshot = new SongSnapshot()
            {
                SongId = song.Id,
                CreatedById = user.Id,
                CreatedOn = DateTimeOffset.Now,
                SongObjectJSON = JSONsnapshot
            };

            if(song.Snapshots.Count >= 5)
            {
                song.PopSnapshot(false);
            }
            song.Snapshots.Add(snapshot);
        }

        public static SongSnapshot PopSnapshot(this Song song, bool descendingOrder)
        {
            SongSnapshot result = null;
            SongSnapshot[] orderedSnapshots;
            if (song.Snapshots.Count > 0)
            {
                if(descendingOrder)
                    orderedSnapshots = song.Snapshots.OrderByDescending(s => s.CreatedOn).ToArray();
                else
                    orderedSnapshots = song.Snapshots.OrderBy(s => s.CreatedOn).ToArray();

                result = orderedSnapshots[0];
                song.Snapshots.Remove(result);
            }
            return result;
        }
    }
}
