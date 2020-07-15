using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// This is the Note, which is associated with a Bar (norsk: Takt)
    /// </summary>
    public class Note : BaseEntity
    {
        /// <summary>
        /// The id of this bar
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The bar it is linked to
        /// and the corresponding BarId
        /// </summary>
        public Bar Bar { get; set; }

        /// <summary>
        /// THe ID for the corresponding Part 
        /// </summary>
        public int BarId { get; set; }

        /// <summary>
        /// Priority of the bar in a spesific part
        /// </summary>
        public byte NoteNumber { get; set; }


        public uint Length { get; set; }

        public string[] NoteValues { get; set; }

        /// <summary>
        /// Empty constructor for Bar
        /// </summary>
        public Note () { }
    }
}
