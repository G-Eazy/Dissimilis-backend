using Dissimilis.DbContext.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// Group entity
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The id of the group
        /// </summary>
        [Key]
        public int Id { get; set; }
         
        /// <summary>
        /// The name of the group
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// The different users and their roles in the group
        /// </summary>
        public ICollection<GroupUser> Users { get; set; } = new List<GroupUser>();

        /// <summary>
        /// List of all shared songs in this group
        /// </summary>
        public ICollection<SongSharedGroup> SharedSongs { get; set; } = new List<SongSharedGroup>();

        /// <summary>
        /// this groups organisation
        /// </summary>
        public Organisation Organisation { get; set; }
        public int OrganisationId { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }
        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }

        /// <summary>
        /// Empty constructor 
        /// </summary>
        public Group() { }

        /// <summary>
        /// Constructor with params
        /// </summary>
        /// <param name="name"></param>
        /// <param name="organisationId"></param>
        /// <param name="address"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <param name="description"></param>
        public Group(string name, int organisationId, string address, string phoneNumber, string email, string description, int createdById)
        {
            Name = name;
            OrganisationId = organisationId;
            Address = address;
            PhoneNumber = phoneNumber;
            EmailAddress = email;
            Description = description;
            CreatedOn = DateTimeOffset.Now;
            CreatedById = createdById;
        }
    }
}
