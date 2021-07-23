using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// the mapping between Group and user with the predefined enum role
    /// </summary>
    public class SongOrganisationTag
    {
   

        /// <summary>
        /// the Group this user has the given role
        /// </summary>
        public Organisation Organisation { get; set; }
        public int OrganisationId { get; set; }

        /// <summary>
        /// the user with the given role in the Group
        /// </summary>
        public Song.Song Song { get; set; }
        public int SongId { get; set; }
    }
}
