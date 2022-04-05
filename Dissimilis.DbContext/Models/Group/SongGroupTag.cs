using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// the mapping between Group and user with the predefined enum role
    /// </summary>
    public class SongGroupTag
    {
 

        /// <summary>
        /// the Group this user has the given role
        /// </summary>
        public Group Group { get; set; }
        public int GroupId { get; set; }

        /// <summary>
        /// the user with the given role in the Group
        /// </summary>
        public Song.Song Song { get; set; }
        public int SongId { get; set; }
    }
}
