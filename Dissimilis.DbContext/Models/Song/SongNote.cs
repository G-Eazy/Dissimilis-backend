using System;
using System.Collections.Generic;
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
            // Notes ordered from low to high pitch
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "H", "Z", "X"
            //"G", "E", "C", "D#", "A#", "H", "A", "D", "F", "F#", "G#", "C#", "Z"
        };

        /// <summary>
        /// The id of this Note
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Priority of the Note in a spesific Bar
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The active chord if a chord is selected
        /// </summary>
        [MaxLength(10)]
        public string ChordName { get; set; }

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
            return NoteValues.Split('|');
            //return GetValidatedNoteValues(NoteValues.Split('|')).ToArray();
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

        public SongNote Clone(bool includeComponentIntervals = true)
        {
            if (includeComponentIntervals)
            {
                return new SongNote()
                    {
                        Length = Length,
                        Position = Position,
                        NoteValues = NoteValues,
                        ChordName = ChordName
                    };
            }
            else
            {
                return new SongNote()
                {
                    Length = Length,
                    Position = Position,
                    NoteValues = String.Join("|", Enumerable.Repeat("X", this.GetNoteValues().Length)),
                    ChordName = ChordName
                };
            }
            
        }

        public SongNote TransposeNoteValues(int transpose = 0)
        {
            var transposedNoteValues = new List<string>();
            var possibleNoteValuesWithoutZ = _possibleNoteValues.Take(_possibleNoteValues.Length - 1).ToArray();



            foreach (var noteValue in GetNoteValues())
            {
                if (noteValue == "Z")
                {
                    continue;
                }

                var index = GetNoteOrderValue(noteValue) + transpose;

                // Map negative transpose-values to positive equivalent
                var transposedIndex = index >= 0 ? index % possibleNoteValuesWithoutZ.Length
                    : (index % possibleNoteValuesWithoutZ.Length) + possibleNoteValuesWithoutZ.Length;

                var valueToAdd = possibleNoteValuesWithoutZ[transposedIndex];
                transposedNoteValues.Add(valueToAdd);
            }

            SetNoteValues(transposedNoteValues.ToArray());

            return this;
        }
    }
}
