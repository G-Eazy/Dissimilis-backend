using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// Entity class f
    /// </summary>
    public class Resource : BaseEntity
    {
        /// <summary>
        /// The Id of this Resource
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// the type of resource a user has, eg. printing, deleting etc
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Empty constructor for Resource
        /// </summary>
        public Resource() { }

        /// <summary>
        /// Contructor for Resource
        /// </summary>
        /// <param name="name"></param>
        public Resource(string name)
        {
            this.Name = name;
        }
    }
}
