using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Extensions.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut
{
    public class NoteDto
    {
        public int? ChordId { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public string ChordName { get; set; }

        /// <summary>
        /// One or of the following values
        /// "G" | "E" | "C"​​ | "D#"​​ | "A#"​​ | "H" | "A"​​ | "D"​​ | "F"​​ | "F#"​​ | "G#"​​ | "C#" | " "
        /// </summary>
        public string[] Notes { get; set; }

        public NoteDto(SongNote songNote)
        {
            if (songNote.Id != 0)
            {
                ChordId = songNote.Id;
            }
            Position = songNote.Position;
            Length = songNote.Length;
            Notes = songNote.GetNoteValues();
            ChordName = songNote.ChordName;
        }

        public NoteDto() { }

        public static SongNote ConvertToSongNote(NoteDto note, SongBar bar)
        {
            return new SongNote()
            {
                Position = note.Position,
                ChordName = note.ChordName,
                Length = note.Length,
                NoteValues = (note.ChordName == null) ? string.Join("|", note.Notes) : string.Join("|", SongNoteExtension.GetNoteValuesFromChordName(note.ChordName)),
                SongBar = bar,
                BarId = bar.Id
            };
        }
    }
}
