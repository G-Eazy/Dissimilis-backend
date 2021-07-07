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
    public class Organisation : ICreatedAndUpdated
    {
        /// <summary>
        /// ID of the organisation
        /// </summary>
        [Key]
        public int Id { get; set; }

        public Guid? MsId { get; set; }

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


        public User CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? CreatedById { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTimeOffset? CreatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public User UpdatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? UpdatedById { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTimeOffset? UpdatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
