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
        /// Boolean value to check if the user is systemAdmin, 
        /// </summary>
        public bool IsSystemAdmin { get; set;}

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

        public ICollection<Song.Song> SongsArranged { get; set; } = new List<Song.Song>();
        public ICollection<Song.Song> SongsCreated { get; set; } = new List<Song.Song>();
        public ICollection<Song.Song> SongsUpdated { get; set; } = new List<Song.Song>();
        public ICollection<SongVoice> SongVoiceCreated { get; set; } = new List<SongVoice>();
        public ICollection<SongVoice> SongVoiceUpdated { get; set; } = new List<SongVoice>();


        public ICollection<Group> GroupsCreated { get; set; } = new List<Group>();

        public ICollection<Organisation> OrganisationsCreated { get; set; } = new List<Organisation>();

        /// <summary>
        /// the different groups this user is in with the corresponding role
        /// </summary>
        public ICollection<GroupUser> Groups { get; set; } = new List<GroupUser>();

        /// <summary>
        /// the different Organisations this user is in with the corresponding role
        /// </summary>
        public ICollection<OrganisationUser> Organisations { get; set; } = new List<OrganisationUser>();

        /// <summary>
        /// the different songs this user has write permission on
        /// </summary>
        public ICollection<SongSharedUser> SongsShared { get; set; } = new List<SongSharedUser>();

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
        public User(string name, string email, int? countryId = null)
        {
            this.Email = email;
            this.Name = name;
            this.CountryId = countryId;
        }
    }
}
