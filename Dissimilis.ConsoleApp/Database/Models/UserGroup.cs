using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class UserGroup
    {
        [Key]
        public int UserGroupId { get; set; }

        /// <summary>
        /// Name of the grouptype for users
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The resource group(s) a user belongs too
        /// </summary>
        public string ResourceGroup { get; set; }

        public UserGroup() { }

        public UserGroup(string groupname)
        {
            this.GroupName = groupname;
        }
    }
}
