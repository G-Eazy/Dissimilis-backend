using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewNoteDTO
    {
        public int BarId { get; set; }
        public byte NoteNumber { get; set; }
        public byte Length { get; set; }
        public string[] NoteValues { get; set; }
    }
}
