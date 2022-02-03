using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.PagedExtensions;

namespace Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn
{
    public class UsersInMyGroupsDto : IPageSelectDto
    {
        /// <summary>
        /// Will search email and name field
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Optional group filter
        /// </summary>
        public int[] GroupFilter { get; set; } = new int[0];

        /// <summary>
        /// Optional organization filter
        /// </summary>
        public int[] OrganizationFilter { get; set; } = new int[0];

        /// <summary>
        /// Default 100
        /// </summary>
        [Range(1, 1000)]
        public int PageSize { get; set; } = 100;

        /// <summary>
        /// Default 1
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
    }
}
