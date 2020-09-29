using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongExtension
    {
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
                var highestBarNumber = songVoice.SongBars.Select(sb => sb.BarNumber)
                    .OrderByDescending(v => v)
                    .FirstOrDefault();

                highestBarNumber++;
                while (songVoice.SongBars.Count < maxBarCount)
                {
                    songVoice.SongBars.Add(new SongBar(highestBarNumber++));
                }

                songVoice.SortBars();
            }
        }

        public static void UpdateAllSongVoices(this Song song, int userId)
        {
            foreach (var songVoice in song.Voices)
            {
                songVoice.SetSongVoiceUpdated(userId);
            }
        }

        public static void RemoveSongBarFromAllVoices(this Song song, int barNumber)
        {
            foreach (var songVoice in song.Voices)
            {
                var barToRemove = songVoice.SongBars.FirstOrDefault(sb => sb.BarNumber == barNumber) ?? songVoice.SongBars.LastOrDefault();

                songVoice.SongBars.Remove(barToRemove);
                songVoice.SortBars();
            }
        }
    }
}
