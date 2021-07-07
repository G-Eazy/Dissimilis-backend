using System.Linq;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Interfaces;
using System;

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
                Voices = song.Voices.Select(v => v.Clone(v.VoiceName)).ToArray()
            };
        }

        public static Song Transpose(this Song song, int transpose = 0)
        {
            song.Voices = song.Voices.Select(v => v.Transpose(transpose)).ToArray();

            return song;
        }

        public static string[] Undo(this Song song)
        {
            return null;
        }

        /// <summary>
        /// This method should be called before any action that updates a song, except when the song itself is created.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="user"></param>
        public static void PerformSnapshot(this Song song, User user)
        {
            string JSONsnapshot = Newtonsoft.Json.JsonConvert.SerializeObject(song);
            Console.WriteLine(JSONsnapshot);
            SongSnapshot snapshot = new SongSnapshot()
            {
                SongId = song.Id,
                Song = song,
                CreatedById = user.Id,
                CreatedOn = DateTimeOffset.Now,
                SongObjectJSON = JSONsnapshot
            };

            if(song.Snapshots.Count > 5)
            {
                song.PopSnapshot();
            }

            song.Snapshots.Add(snapshot);
        }

        public static SongSnapshot PopSnapshot(this Song song)
        {
            SongSnapshot result = null;

            if(song.Snapshots.Count > 0)
            {
                var orderedSnapshots = song.Snapshots.OrderBy(s => s.CreatedOn).ToArray();
                result = orderedSnapshots[0];

                song.Snapshots.Remove(result);
            }
            return result;
        }
    }
}
