using System;
using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongByIdDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }

        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public string ArrangerName { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public SongVoiceDto[] Voices { get; set; }

        public SongByIdDto(Song song)
        {
            SongId = song.Id;
            Title = song.Title;
            ArrangerName = song.Arranger?.Name ;
            UpdatedOn = song.UpdatedOn;
            Numerator = song.Numerator;
            Denominator = song.Denominator;
            Console.WriteLine($"Songvoices:\n{song.Voices.ToString()}");
            Voices = song.Voices
                .Select(p => new SongVoiceDto(p))
                .OrderBy(p => p.PartNumber)
                .ToArray();
        }
    }
}
