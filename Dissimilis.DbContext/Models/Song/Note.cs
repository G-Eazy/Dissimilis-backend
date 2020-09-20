using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// This is the Note, which is associated with a Bar (norsk: Takt)
    /// </summary>
    public class Note 
    {
        /// <summary>
        /// The id of this Note
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Priority of the Note in a spesific Bar
        /// </summary>
        public int NoteNumber { get; set; }

        /// <summary>
        /// The lenght of this note/chord
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Either a single note, two notes or chords ["C", "F", "A"]
        /// Possible values:
        /// "G" | "E" | "C"​​ | "D#"​​ | "A#"​​ | "H" | "A"​​ | "D"​​ | "F"​​ | "F#"​​ | "G#"​​ | "C#" | " "
        /// </summary>
        [MaxLength(100)]
        public string[] NoteValues { get; set; }

        /// <summary>
        /// The bar it is linked to
        /// and the corresponding BarId
        /// </summary>
        public Bar Bar { get; set; }
        public int BarId { get; set; }

       
    }
}
