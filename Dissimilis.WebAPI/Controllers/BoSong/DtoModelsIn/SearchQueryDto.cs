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
        public int MaxNumberOfSongs { get; set; } = 100;

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
        public int[] IncludedGroupIdArray { get; set; } = System.Array.Empty<int>();

        /// <summary>
        /// OrgIds of what organisations to show songs for
        /// </summary>
        public int[] IncludedOrganisationIdArray { get; set; } = System.Array.Empty<int>();

        /// <summary>
        /// show songs shared with user, that the user has write permission on
        /// </summary>
        public bool IncludeSharedWithUser { get; set; }

        /// <summary>
        /// If true returns all public songs in the system.
        /// Only overrides IncludedGroupIdArray, IncludedOrganisationIdArray and IncludeSharedWithUser. (not Title, ArrangerId)
        /// </summary>
        public bool IncludeAll { get; set; } = true;
    }
}
