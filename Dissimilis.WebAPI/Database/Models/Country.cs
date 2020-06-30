using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Country
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The name of the country
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Country code for current country (eg. Norway == No)
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Empty constructor for Country
        /// </summary>
        public Country() { }

        /// <summary>
        /// Constructor for Country with parameteres
        /// </summary>
        /// <param name="country"></param>
        /// <param name="language"></param>
        public Country(string country, string language)
        {
            this.CountryName = country;
            this.Language = language;
        }
    }
}
