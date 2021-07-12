﻿using System.Linq;
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

        /*private static List<SongVoice> _GetVoicesFromDeserialisedSong(Song song, JObject deserialisedObj)
        {
            /*SongSnapshotDto songSnapshot = new SongSnapshotDto()
            {
                SongTitle = deserialisedObj["Title"].Value<string>(),
                SongVoices = deserialisedObj["Voices"].Value<SongVoiceDto[]>()
            };
            List<SongVoice> voices = new List<SongVoice>();

            foreach(SongVoiceDto voiceDto in songSnapshot.SongVoices)
            {
                SongVoice voice = SongVoiceDto.ConvertToSongVoice(voiceDto, DateTimeOffset.Now, snapshot.CreatedBy, snapshot.CreatedById, song);
                voices.Add(voice);
                foreach(BarDto barDto in voiceDto.Bars)
                {
                    SongBar bar = BarDto.ConvertToSongBar(barDto);
                    voice.SongBars.Add(bar);
                    foreach(NoteDto noteDto in barDto.Chords)
                    {
                        SongNote note = NoteDto.ConvertToSongNote(noteDto, bar);
                        bar.Notes.Add(note);
                    }
                }
            }
            List<SongVoice> voices = new List<SongVoice>();
            SongVoiceDto[] voiceDtos = deserialisedObj["Voices"].Value<SongVoiceDto[]>();
            User createdBy = deserialisedObj[""]

            foreach (SongVoiceDto voiceDto in voiceDtos)
            {
                SongVoice voice = SongVoiceDto.ConvertToSongVoice(voiceDto, DateTimeOffset.Now, deserialisedObj.CreatedBy, snapshot.CreatedById, song);
                voices.Add(voice);
                foreach (BarDto barDto in voiceDto.Bars)
                {
                    SongBar bar = BarDto.ConvertToSongBar(barDto);
                    voice.SongBars.Add(bar);
                    foreach (NoteDto noteDto in barDto.Chords)
                    {
                        SongNote note = NoteDto.ConvertToSongNote(noteDto, bar);
                        bar.Notes.Add(note);
                    }
                }
            }
            return voices;

        }*/

        public static void Undo(this Song song)
        {
            Console.WriteLine(song.Snapshots.Count);
            SongSnapshot snapshot = song.PopSnapshot(true);
            SongSnapshotDto snapshotDto = Newtonsoft.Json.JsonConvert.DeserializeObject<SongSnapshotDto>(snapshot.SongObjectJSON);

            JObject deserialisedSong = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(snapshot.SongObjectJSON);

            song.Title = deserialisedSong["Title"].Value<string>();
            song.UpdatedBy = snapshot.CreatedBy;
            song.UpdatedOn = DateTimeOffset.Now;

            // Constructing voices from deserialised json
            List<SongVoice> voices = new List<SongVoice>();
            Console.WriteLine(snapshot.SongObjectJSON);
            JArray voiceJSONs = deserialisedSong["Voices"].Value<JArray>();
            User createdBy = song.CreatedBy;

            foreach (var voiceJSON in voiceJSONs)
            {
                SongVoiceDto voiceDto = SongVoiceDto.JsonToSongVoiceDto(voiceJSON);
                SongVoice voice = song.Voices.FirstOrDefault(v => v.VoiceNumber == voiceDto.PartNumber);
                if(voice == null)
                    voice = SongVoiceDto.ConvertToSongVoice(voiceDto, DateTimeOffset.Now, snapshot.CreatedById);

                List<SongBar> newBars = new List<SongBar>();
                foreach (var barJSON in voiceJSON["Bars"])
                {
                    BarDto barDto = BarDto.JsonToBarDto(barJSON);
                    SongBar bar = voice.SongBars.FirstOrDefault(b => b.Position == barDto.Position);
                    if(bar == null)
                        bar = BarDto.ConvertToSongBar(barDto);

                    List<SongNote> newNotes = new List<SongNote>();
                    foreach (var noteJSON in barJSON["Chords"])
                    {
                        NoteDto noteDto = NoteDto.JsonToNoteDto(noteJSON);
                        bool emptyNote = noteDto.Notes[0] == "Z";
                        if (emptyNote)
                        {
                            for(int i = noteDto.Position; i < noteDto.Position + noteDto.Length; i++)
                            {
                                var noteToBeRemoved = bar.Notes.FirstOrDefault(n => n.Position == i);
                                bar.Notes.Remove(noteToBeRemoved);
                            }
                        }
                        else
                        {
                            SongNote note = bar.Notes.FirstOrDefault(n => n.Position == noteDto.Position);
                            if (note == null)
                                note = NoteDto.ConvertToSongNote(noteDto, bar);
                            note.SetNoteValues(noteDto.Notes);
                            newNotes.Add(note);
                        }
                        
                    }
                    bar.Notes = newNotes;
                    Array.ForEach(bar.Notes.ToArray(), Console.WriteLine);
                    newBars.Add(bar);
                }
                voice.SongBars = newBars;
                voices.Add(voice);
            }
            song.Voices = voices;
        }

        /// <summary>
        /// This method should be called before any action that updates a song, except when the song itself is created.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="oldSong"></param>
        /// <param name="user"></param>
        public static void PerformSnapshot(this Song song, User user)
        {
            var s = new SongByIdDto(song);
            string JSONsnapshot = Newtonsoft.Json.JsonConvert.SerializeObject(new {
                Title = s.Title,
                Voices = s.Voices
            });
            Console.WriteLine($"Snapshot:\n{JSONsnapshot}");
            SongSnapshot snapshot = new SongSnapshot()
            {
                SongId = s.SongId,
                CreatedById = user.Id,
                CreatedOn = DateTimeOffset.Now,
                SongObjectJSON = JSONsnapshot
            };
            if(song.Snapshots.Count > 5)
            {
                song.PopSnapshot(false);
            }
            song.Snapshots.Add(snapshot);
            Console.WriteLine($"Snapshot object: {snapshot}");
        }

        public static SongSnapshot PopSnapshot(this Song song, bool descendingOrder)
        {
            SongSnapshot result = null;
            Console.WriteLine($"Song currently has this many snapshots: {song.Snapshots.Count}");
            SongSnapshot[] orderedSnapshots;
            if (song.Snapshots.Count > 0)
            {
                if(descendingOrder)
                    orderedSnapshots = song.Snapshots.OrderByDescending(s => s.CreatedOn).ToArray();
                else
                    orderedSnapshots = song.Snapshots.OrderBy(s => s.CreatedOn).ToArray();
                Console.WriteLine($"Length of ordered snapshots: {orderedSnapshots.Length}");
                result = orderedSnapshots[0];

                song.Snapshots.Remove(result);
            }
            Console.WriteLine($"Popped snapshot: {result}");
            return result;
        }
    }
}
