using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// Note
    /// </summary>
    public class SongNote
    {
        internal static string[] _possibleNoteValues = new string[]
        {
            "G", "E", "C", "D#", "A#", "H", "A", "D", "F", "F#", "G#", "C#", " "
        };

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
        public string NoteValues { get; set; }

        /// <summary>
        /// The bar it is linked to
        /// and the corresponding BarId
        /// </summary>
        public SongBar SongBar { get; set; }
        public int BarId { get; set; }


        public string[] GetNoteValues()
        {
            return GetValidatedNoteValues(NoteValues.Split('|')).ToArray();
        }

        public void SetNoteValues(string[] noteValues)
        {
            NoteValues = string.Join("|", GetValidatedNoteValues(noteValues));
        }

        public static string[] GetValidatedNoteValues(string[] input)
        {
            return input
                .Select(v => v.ToUpper().Trim())
                .Distinct()
                .Where(v => _possibleNoteValues.Contains(v))
                .ToArray();
        }
    }
}
