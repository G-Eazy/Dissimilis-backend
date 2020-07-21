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

        public NoteDTO(int id, int barid, byte notenumber, byte length, string[] values)
        {
            this.Id = id;
            base.BarId = barid;
            base.NoteNumber = notenumber;
            base.Length = length;
            base.NoteValues = values;
        }
    }
}
