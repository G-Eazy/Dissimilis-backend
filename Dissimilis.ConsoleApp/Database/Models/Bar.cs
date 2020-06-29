using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Bar
    {
        /// <summary>
        /// The id of this bar
        /// </summary>
        [Key]
        public int BarId { get; set; }

        /// <summary>
        /// The song it is linked to
        /// and the corresponding SongId
        /// </summary>
        public Part Part { get; }

        public int PartId { get; set; }
    }
}
