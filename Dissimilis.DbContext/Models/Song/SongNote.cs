using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// Note
    /// </summary>
    public class SongNote
    {
        internal static string[] _possibleNoteValues = new string[]
        {
            "G", "E", "C", "D#", "A#", "H", "A", "D", "F", "F#", "G#", "C#", "Z"
        };

        /// <summary>
        /// The id of this Note
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Priority of the Note in a spesific Bar
        /// </summary>
        public int Postition { get; set; }

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

        public void SetNoteValues(string[] noteValues, bool throwValidationException = true)
        {
            var validatedNotes = GetValidatedNoteValues(noteValues, false);
            if (throwValidationException && !validatedNotes.Any())
            {
                throw new ValidationException("No valid notes passed in");
            }

            NoteValues = string.Join("|", validatedNotes);
        }

        public static int GetNoteOrderValue(string noteValue)
        {
            try
            {
                return Array.IndexOf(_possibleNoteValues, noteValue);
            }
            catch
            {
                return -1;
            }
        }

        public static string[] GetValidatedNoteValues(string[] input, bool includeZ = true)
        {
            var result = input
                .Select(v => v.ToUpper().Trim())
                .Distinct()
                .Where(v => _possibleNoteValues.Contains(v))
                .ToArray();

            if (!includeZ)
            {
                result = result.Where(n => n != "Z").ToArray();
            }

            return result
                //.OrderBy(GetNoteOrderValue) // frontend did not like this at this point
                .ToArray();
        }
    }
}
