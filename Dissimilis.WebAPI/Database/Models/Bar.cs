﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// This is the bar, which is associated with a Part (norsk: Stemme)
    /// </summary>
    public class Bar : BaseEntity
    {
        /// <summary>
        /// The id of this bar
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The part it is linked to
        /// and the corresponding PartId
        /// </summary>
        public Part Part { get; set; }

        /// <summary>
        /// THe ID for the corresponding Part 
        /// </summary>
        public int PartId { get; set; }

        /// <summary>
        /// the actual ABC notation for this bar
        /// </summary>
        public string Notation { get; set; }

        /// <summary>
        /// Priority of the bar in a spesific part
        /// </summary>
        public byte BarNumber { get; set; }

        /// <summary>
        /// Empty constructor for Bar
        /// </summary>
        public Bar () { }
    }
}
