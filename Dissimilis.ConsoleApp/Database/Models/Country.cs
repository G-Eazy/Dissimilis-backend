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

        public Country() { }

        public Country(string country)
        {
            this.CountryName = country;
        }
    }
}
