using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// User entity that contains: 
    /// Id, email, name, DOB, org and country
    /// </summary>
    public class User
    {
        /// <summary>
        /// The Id of the user
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The microsoft Id of this user
        /// </summary>
        [MaxLength(200)]
        public string MsId { get; set; }


        /// <summary>
        /// Email address of user
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// Country the user is from or belongs to
        /// </summary>
        public Country Country { get; set; }
        public int? CountryId { get; set; }
        /// <summary>
        /// The Organisation object associated with this user
        /// </summary>
        public Organisation Organisation { get; set; }
        public int? OrganisationId { get; set; }

        public ICollection<Song.Song> SongsArranged { get; set; } = new List<Song.Song>();
        public ICollection<Song.Song> SongsCreated { get; set; } = new List<Song.Song>();
        public ICollection<Song.Song> SongsUpdated { get; set; } = new List<Song.Song>();
        public ICollection<SongVoice> SongVoiceCreated { get; set; } = new List<SongVoice>();
        public ICollection<SongVoice> SongVoiceUpdated { get; set; } = new List<SongVoice>();


        /// <summary>
        /// Empty constructor for User
        /// </summary>
        public User() { }

        /// <summary>
        /// Constructor for User
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="organisationId"></param>
        /// <param name="countryId"></param>
        public User(string name, string email, int? organisationId = null, int? countryId = null)
        {
            this.Email = email;
            this.OrganisationId = organisationId;
            this.Name = name;
            this.CountryId = countryId;
        }
    }
}
