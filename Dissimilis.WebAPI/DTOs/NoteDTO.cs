using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NoteDTO : NewNoteDTO
    {
        public int Id { get; set; }
        
        public NoteDTO() { }

        public NoteDTO(INote note)
        {
            if (note is null)
                throw new ArgumentNullException(nameof(note));

            this.Id = note.Id;
            base.BarId = note.BarId;
            base.NoteNumber = note.NoteNumber;
            base.Length = note.Length;
            base.Notes = note.NoteValues;
        }
    }
}
