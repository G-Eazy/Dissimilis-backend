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
                return string.Join(" ",parts);
            }

            return songVoiceInstrumentName + " 1";
        }

        public static SongVoice Clone(this SongVoice songVoice, User user, Instrument instrument, int voiceNumber = -1)
        {
            var newSongVoice = new SongVoice()
            {
                SongBars = songVoice.SongBars.Select(b => b.Clone()).ToArray(),
                IsMainVoice = false,
                Instrument = instrument,
                VoiceNumber = voiceNumber == -1 ? songVoice.VoiceNumber + 1 : voiceNumber
            };

            newSongVoice.SetCreated(user);

            return newSongVoice;
        }
    }
}
