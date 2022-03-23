using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    ///the songs shared among users
    /// </summary>
    public class SongSharedUser
    {
        /// <summary>
        /// the User with writing permissions on this song 
        /// </summary>
        public User User { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// the shared song
        /// </summary>
        public Song.Song Song { get; set; }
        public int SongId { get; set; }
    }
}
