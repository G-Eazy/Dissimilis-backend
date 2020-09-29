using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the country
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Empty constructor for Country
        /// </summary>
        public Country() { }

        /// <summary>
        /// Constructor for Country with parameteres
        /// </summary>
        /// <param name="country"></param>
        public Country(string country)
        {
            this.Name = country;
        }
    }
}
