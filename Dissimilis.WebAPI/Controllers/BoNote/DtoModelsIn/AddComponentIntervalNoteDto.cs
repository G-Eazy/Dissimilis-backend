using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn
{
    public class AddComponentIntervalNoteDto
    {
        public string ChordName { get; set; }
        public int NotePosition { get; set; }
        public int Length { get; set; }
        public int IntervalPosition { get; set; }
    }
}
