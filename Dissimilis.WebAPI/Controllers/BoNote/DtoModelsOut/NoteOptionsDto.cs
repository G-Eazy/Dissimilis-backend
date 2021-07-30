using System.Collections.Generic;

namespace Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut
{
    public class NoteOptionsDto
    {
        public List<string> SingleNoteOptions { get; set; }
        public Dictionary<string, string[]> ChordOptions { get; set; }
    }
}
