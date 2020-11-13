using Dissimilis.DbContext.Models.Song;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongNoteExtention
    {
        internal static string[] _possibleNoteValues = new string[]
        {
            // Notes ordered from low to high pitch
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "H", "Z"
        };

        public static SongNote TransposeNoteValues(this SongNote songNote, int transpose = 0)
        {
            var transposedNote = new SongNote()
            {
                Length = songNote.Length,
                Postition = songNote.Postition,
                NoteValues = songNote.NoteValues
            };

            var transposedNoteValues = new List<string>();
            var possibleNoteValuesWithoutZ = _possibleNoteValues.Take(_possibleNoteValues.Length - 1).ToArray();

            foreach (var noteValue in transposedNote.GetNoteValues())
            {
                if (noteValue == "Z")
                {
                    transposedNoteValues.Add(noteValue);
                }
                else
                {
                    var index = SongNote.GetNoteOrderValue(noteValue);

                    var valueToAdd = possibleNoteValuesWithoutZ[index + transpose % possibleNoteValuesWithoutZ.Length];
                    transposedNoteValues.Add(valueToAdd);
                }
            }

            transposedNote.SetNoteValues(transposedNoteValues.ToArray());

            return transposedNote;
        }
    }
}
