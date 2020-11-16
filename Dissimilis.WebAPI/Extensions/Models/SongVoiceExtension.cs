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
                VoiceNumber = voiceNumber == -1 ? songVoice.VoiceNumber + 1 : voiceNumber
            };

            if (newSongVoice.VoiceNumber != songVoice.VoiceNumber)
            {
                newSongVoice.IsMainVoice = false;
            }

            newSongVoice.SetCreated(user?.Id ?? songVoice.CreatedById);

            return newSongVoice;
        }

        public static SongVoice Transpose(this SongVoice songVoice, int transpose = 0)
        {
            var newSongVoice = new SongVoice()
            {
                SongBars = songVoice.SongBars.Select(b => b.Transpose(transpose)).ToArray(),
                Instrument = songVoice.Instrument,
                VoiceNumber = songVoice.VoiceNumber
            };

            return newSongVoice;
        }
    }
}
