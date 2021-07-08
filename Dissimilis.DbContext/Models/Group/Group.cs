using Dissimilis.DbContext.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// Group entity
    /// </summary>
    public class Group : ICreatedAndUpdated
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
        public string Name { get; set; }

        /// <summary>
        /// The different users and their roles in the group
        /// </summary>
        public ICollection<GroupUser> Users { get; set; } = new List<GroupUser>();

        /// <summary>
        /// this groups organisation
        /// </summary>
        public Organisation Organisation { get; set; }
        public int? OrganisationId { get; set; }

        /// <summary>
        /// The info description of the group
        /// </summary>
        public string GroupInfo { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }
        public User UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }

        /// <summary>
        /// Empty constructor 
        /// </summary>
        public Group() { }

        /// <summary>
        /// Constructor for User
        /// </summary>
        /// <param name="name"></param>
        /// <param name="adminUser"></param>
        /// <param name="organisationId"></param>
        public Group(GroupUser adminUser, int? organisationId, string name)
        {
            Name = name;
            Users.Add(adminUser);
            OrganisationId = organisationId;
           
        }
    }
}
