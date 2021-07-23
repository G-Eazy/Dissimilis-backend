using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn
{
    public class RemoveComponentIntervalNoteDto
    {
        public bool DeleteOnLastIntervalRemoved { get; set; } = false;
        public int SongNotePosition { get; set; }
        public int IntervalPosition { get; set; }
    }
}
