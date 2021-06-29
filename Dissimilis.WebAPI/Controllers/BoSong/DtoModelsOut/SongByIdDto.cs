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
        public string SongNotes { get; set; }

        public int? Speed { get; set; }

        public int? DegreeOfDifficulty { get; set; }

        public SongByIdDto(Song song)
        {
            SongId = song.Id;
            Title = song.Title;
            ArrangerName = song.Arranger?.Name ;
            UpdatedOn = song.UpdatedOn;
            Numerator = song.Numerator;
            Denominator = song.Denominator;
            Speed = song.Speed;
            DegreeOfDifficulty = song.DegreeOfDifficulty;
            SongNotes = song.SongNotes;

            Voices = song.Voices
                .Select(p => new SongVoiceDto(p))
                .OrderBy(p => p.PartNumber)
                .ToArray();
        }
    }
}
