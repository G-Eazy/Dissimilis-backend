namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn
{
    public class SearchQueryDto
    {

        public string Title { get; set; }

        public int? ArrangerId { get; set; }

        public int? Num { get; set; }

        public bool OrderByDateTime { get; set; } = false;

    }
}
