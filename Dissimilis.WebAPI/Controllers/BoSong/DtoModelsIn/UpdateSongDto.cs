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

        // <summary>
        // The speed of the song
        // ( set a max range of 256) 
        // </summary>
        //[Range(0, 256)]
        //public int? Speed { get; set; }
        //
        ///// <summary>
        ///// how hard the song is to play
        ///// (could change the range)
        ///// </summary>
        //[Range(1, 10)]
        //public int? DegreeOfDifficulty { get; set; }
        //
    }
}
