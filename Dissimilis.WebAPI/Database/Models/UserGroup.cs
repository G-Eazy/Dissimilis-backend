using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dissimilis.WebAPI.Database.Models
{
    public class UserGroup : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the grouptype for users
        /// </summary>
        [Required]
        public string Name { get; set; }


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
            this.Name = groupname;
        }
    }
}
