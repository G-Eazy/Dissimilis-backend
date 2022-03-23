using Dissimilis.DbContext.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn
{
    public class UpdateSongDto 
    {
        public string Title { get; set; }

        //[Range(-10, 10)]
        //public int Numerator { get; set; }
        //[Range(1, 10)]
        //public int Denominator { get; set; }
        //public string? SongNotes { get; set; }
        [Range(0, 256)]
        public int? Speed { get; set; }

        [Range(1,10)]
        public int? DegreeOfDifficulty { get; set; }
        [MaxLength(250)]        
        public string SongNotes { get; set; }

        public string Composer { get; set; }
    }
}
