using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Resource
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// the type of resource a user has, eg. printing, deleting etc
        /// </summary>
        public string ResourceType { get; set; }
    }
}
