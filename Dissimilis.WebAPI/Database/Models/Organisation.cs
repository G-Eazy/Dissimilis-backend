using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Models
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
        public string Name { get; set; }

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
