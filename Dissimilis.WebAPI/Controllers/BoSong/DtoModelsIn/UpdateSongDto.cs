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

        public int? Speed { get; set; }

        public int? DegreeOfDifficulty { get; set; }
        
        public string SongNotes { get; set; }
    }
}
