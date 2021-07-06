﻿using System.Collections.Generic;
using System.Linq;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Extensions.Interfaces;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongVoiceExtension
    {
        public static void SortBars(this SongVoice songVoice)
        {
            var number = 1;
            var orderedList = songVoice.SongBars.OrderBy(b => b.Position);
            foreach (var songBar in orderedList)
            {
                songBar.Position = number++;
            }
        }

        public static void SetSongVoiceUpdated(this SongVoice songVoice, int userId)
        {
            songVoice.SetUpdated(userId);
            songVoice.Song.SetUpdated(userId);
        }

        public static string GetNextSongVoiceName(this string songVoiceInstrumentName)
        {
            var parts = songVoiceInstrumentName.Split(' ').ToList();
            var lastPart = parts.Last();
            if (!string.IsNullOrWhiteSpace(lastPart) && int.TryParse(lastPart, out var result))
            {
                parts.Remove(lastPart);
                parts.Add((result + 1).ToString());
                return string.Join(" ", parts);
            }

            return songVoiceInstrumentName + " 1";
        }

        public static SongVoice Clone(this SongVoice songVoice, User user = null, Instrument instrument = null, int voiceNumber = -1)
        {
            var newSongVoice = new SongVoice()
            {
                SongBars = songVoice.SongBars.Select(b => b.Clone()).ToArray(),
                Instrument = instrument ?? songVoice.Instrument,
                VoiceNumber = voiceNumber == -1 ? songVoice.VoiceNumber : voiceNumber + 1
            };

            if (newSongVoice.VoiceNumber != songVoice.VoiceNumber)
            {
                newSongVoice.IsMainVoice = false;
            } 
            else
            {
                newSongVoice.IsMainVoice = songVoice.IsMainVoice;
            }

            newSongVoice.SetCreated(user?.Id ?? songVoice.CreatedById);

            return newSongVoice;
        }

        public static SongVoice AddComponentInterval(this SongVoice songVoice, int intervalPosition)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                    bar.AddComponentInterval(intervalPosition)
                ).ToArray();
            return songVoice;
        }

        public static SongVoice RemoveComponentInterval(this SongVoice songVoice, int intervalPosition)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                    bar.RemoveComponentInterval(intervalPosition)
                ).ToArray();
            return songVoice;
        }

        public static SongVoice DuplicateAllChords(this SongVoice songVoice, SongVoice sourceSongVoice, bool includeComponentIntervals = true)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
            {
                var sourceBar = sourceSongVoice.SongBars.First(srcBar => srcBar.Position == bar.Position);
                return bar.DuplicateAllChords(sourceBar, includeComponentIntervals);
            }).ToList();
            return songVoice;
        }

        public static SongVoice Transpose(this SongVoice songVoice, int transpose = 0)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                    bar.Transpose(transpose)
                ).ToArray();
            return songVoice;
        }
    }
}
