using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NoteDTO
    {
        public int Id { get; set; }
        public int BarId { get; set; }
        public byte NoteNumber { get; set; }
        public byte Length { get; set; }
        public string[] NoteValues { get; set; }

        public NoteDTO() { }

        public NoteDTO(int id, int barid, byte notenumber, byte length, string[] values)
        {
            this.Id = id;
            this.BarId = barid;
            this.NoteNumber = notenumber;
            this.Length = length;
            this.NoteValues = values;
        }
    }
}
