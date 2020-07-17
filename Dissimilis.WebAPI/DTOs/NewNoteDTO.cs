using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewNoteDTO
    {
        public int Id { get; set; }
        public int Length { get; set; }
        public string[] NoteValues { get; set; }
    }
}
