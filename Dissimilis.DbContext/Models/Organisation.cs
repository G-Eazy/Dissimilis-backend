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

        public Guid? MsId { get; set; }

        /// <summary>
        /// The name of the organisation
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Empty constructor 
        /// </summary>
        public Organisation() { }

        /// <summary>
        /// Constructor that takes a name of the organisation
        /// </summary>
        /// <param name="name"></param>
        public Organisation(string name)
        {
            this.Name = name;
        }
    }
}
