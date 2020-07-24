using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewBarDTO
    {
        public byte BarNumber { get; set; }
        public int PartId { get; set; }
        public bool RepBefore { get; set; } 
        public bool RepAfter { get; set; } 
        public byte? House { get; set; }
        public NewNoteDTO[] ChordsAndNotes { get; set; }

        public NewBarDTO() { }

    }
}
