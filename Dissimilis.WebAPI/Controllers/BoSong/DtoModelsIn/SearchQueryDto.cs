namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn
{
    public class SearchQueryDto
    {
        /// <summary>
        /// the title of the song searching for
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The id of the song arranger
        /// </summary>
        public int? ArrangerId { get; set; }

        /// <summary>
        /// The number of songs to return
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// What to order the songs by
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// true if orderDescendig, false for ascending
        /// </summary>
        public bool OrderDescending { get; set; }

        /// <summary>
        /// groupIds of what groups to show songs for
        /// </summary>
        public int?[] GroupId { get; set; }

        /// <summary>
        /// OrgIds of what organisations to show songs for
        /// </summary>
        public int?[] OrgId { get; set; }

        /// <summary>
        /// show songs shared with you, that you have write permission on
        /// </summary>
        public bool? SharedByUser { get; set; }
    }
}
