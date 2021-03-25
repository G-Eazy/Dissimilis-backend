﻿using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class NoteDto
    {
        public int? ChordId { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public string ActiveChord { get; set; }

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
            ActiveChord = songNote.ActiveChord;
        }
    }
}
