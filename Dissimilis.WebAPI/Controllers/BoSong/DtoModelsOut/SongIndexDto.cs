using System;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongIndexDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }

        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public string ArrangerEmail { get; set; }
        public string ArrangerName { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }

        public int? Speed { get; set; }

        public int? DegreeOfDifficulty { get; set; }
        public bool CurrentUserHasWriteAccess { get; set; }

        public SongIndexDto(Song song, bool currentUserHasWriteAccess)
        {
            SongId = song.Id;
            Title = song.Title;
            ArrangerEmail = song.Arranger?.Email;
            ArrangerName = song.Arranger?.Name;
            UpdatedOn = song.UpdatedOn;
            Numerator = song.Numerator;
            Denominator = song.Denominator;
            Speed = song.Speed;
            DegreeOfDifficulty = song.DegreeOfDifficulty;
            CurrentUserHasWriteAccess = currentUserHasWriteAccess;
        }
    }
}
