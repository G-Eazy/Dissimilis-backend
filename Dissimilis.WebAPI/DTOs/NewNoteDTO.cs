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

        public NewNoteDTO() { }

        public NewNoteDTO(int barId, byte noteNumber, byte length, string[] noteValues)
        {
            this.BarId = barId;
            this.NoteNumber = NoteNumber;
            this.Length = length;
            this.NoteValues = noteValues;
        } 
    }
}
