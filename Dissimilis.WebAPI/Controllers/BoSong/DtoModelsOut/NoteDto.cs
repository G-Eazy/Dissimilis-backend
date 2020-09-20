using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class NoteDto 
    {
        public int NoteId { get; set; }
        public int NoteNumber { get; set; }
        public int Length { get; set; }

        /// <summary>
        /// One or of the following values
        /// "G" | "E" | "C"​​ | "D#"​​ | "A#"​​ | "H" | "A"​​ | "D"​​ | "F"​​ | "F#"​​ | "G#"​​ | "C#" | " "
        /// </summary>
        public string[] Notes { get; set; }

        public NoteDto(Note note)
        {
            NoteId = note.Id;
            NoteNumber = note.NoteNumber;
            Length = note.Length;
            Notes = note.NoteValues;
        }
    }
}
