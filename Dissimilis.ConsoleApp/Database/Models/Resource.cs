using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        /// <summary>
        /// the type of resource a user has, eg. printing, deleting etc
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// The collection of user groups that uses this resource
        /// </summary>
        public ICollection<UserGroup> Usergroups { get; set; }

        /// <summary>
        /// Empty constructor for Resource
        /// </summary>
        public Resource() { }

        /// <summary>
        /// Contructor for Resource
        /// </summary>
        /// <param name="ResourceType"></param>
        public Resource(string ResourceType)
        {
            this.ResourceType = ResourceType;
        }
    }
}
