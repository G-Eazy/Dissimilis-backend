using Dissimilis.DbContext.Models.Song;
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

        public static NoteDto JsonToNoteDto(JToken json)
        {
            int id = (json["ChordId"].Value<string>() != null) ? int.Parse(json["ChordId"].Value<string>()) : 0;
            List<string> notes = new List<string>();
            foreach (var token in json["Notes"])
            { 
                notes.Add(token.Value<string>());
            }
            return new NoteDto()
            {
                ChordId = (id != 0) ? id : null,
                Position = int.Parse(json["Position"].Value<string>()),
                Length = int.Parse(json["Length"].Value<string>()),
                Notes = notes.ToArray(),
                ChordName = (json["ChordName"].Value<string>() != null) ? json["ChordName"].Value<string>() : null
            };
        }

        public static SongNote ConvertToSongNote(NoteDto note, SongBar bar)
        {
            return new SongNote()
            {
                Id = (note.ChordId != null) ? (int)note.ChordId : 0,
                Position = note.Position,
                ChordName = note.ChordName,
                Length = note.Length,
                NoteValues = string.Concat(note.Notes),
                SongBar = bar,
                BarId = bar.Id
            };
        }
    }
}
