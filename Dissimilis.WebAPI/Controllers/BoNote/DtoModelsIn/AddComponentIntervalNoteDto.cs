using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn
{
    public class AddComponentIntervalNoteDto
    {
        [MaxLength(10)]
        public string ChordName { get; set; }
        [MinLength(1)]
        [MaxLength(100)]
        public string[] Notes { get; set; } = Array.Empty<string>();
        [Range(0, int.MaxValue)]
        public int NotePosition { get; set; }
        [Range(1, int.MaxValue)]
        public int Length { get; set; }
        [Range(0, int.MaxValue)]
        public int IntervalPosition { get; set; }
    }
}
