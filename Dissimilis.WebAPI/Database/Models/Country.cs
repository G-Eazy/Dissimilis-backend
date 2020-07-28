using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dissimilis.WebAPI.Database.Models
{
    public class Country : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the country
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

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
