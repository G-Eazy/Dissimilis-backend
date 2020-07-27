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
    public class Note : BaseEntity, INote
    {
        /// <summary>
        /// The id of this Note
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
        /// Priority of the Note in a spesific Bar
        /// </summary>
        public ushort NoteNumber { get; set; }

        /// <summary>
        /// The lenght of this note/chord
        /// </summary>
        public byte Length { get; set; }

        /// <summary>
        /// Either a single note, two notes or chords ["C", "F", "A"]
        /// </summary>
        public string[] NoteValues { get; set; }

        /// <summary>
        /// Empty constructor for Note
        /// </summary>
        public Note() { }
    }
}
