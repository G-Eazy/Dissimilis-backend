using Dissimilis.DbContext.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// Entity class for organisation
    /// Contains Id and Name
    /// </summary>
    public class Organisation
    {
        /// <summary>
        /// ID of the organisation
        /// </summary> 
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The name of the organisation
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// The users in the organisation with their roles
        /// </summary>
        public ICollection<OrganisationUser> Users { get; set; } = new List<OrganisationUser>();
        /// <summary>
        /// The Groups in the organisation
        /// </summary>
        public ICollection<Group> Groups { get; set; } = new List<Group>();

        /// <summary>
        /// List of all shared songs in this organisation
        /// </summary>
        public ICollection<SongSharedOrganisation> SharedSongs { get; set; } = new List<SongSharedOrganisation>();

        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public DateTimeOffset? CreatedOn { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Empty constructor 
        /// </summary>
        public Organisation() { }

        /// <summary>
        /// Constructor that takes a name of the organisation
        /// </summary>
        /// <param name="name"></param>
        /// <param adminUser="adminUser"></param>
        public Organisation(string name, OrganisationUser adminUser)
        {
            Name = name;
            Users.Add(adminUser);
            
        }
    }
}
