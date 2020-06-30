using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dissimilis.WebAPI.Database.Models
{
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the grouptype for users
        /// </summary>
        [Required]
        public string GroupName { get; set; }

        /// <summary>
        /// The resource group(s) a user belongs too
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// The collection of users belonging to this usergroup
        /// </summary>
        public ICollection<User> Users { get; set; }

        /// <summary>
        /// Empty constructor for UserGroup
        /// </summary>
        public UserGroup() { }

        /// <summary>
        /// Constructor for UserGroup
        /// </summary>
        /// <param name="groupname"></param>
        public UserGroup(string groupname)
        {
            this.GroupName = groupname;
        }
    }
}
