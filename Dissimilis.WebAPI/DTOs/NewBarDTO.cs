using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewBarDTO : IDTO
    {
        public byte BarNumber { get; set; }
        public int PartId { get; set; }
        public bool RepBefore { get; set; } 
        public bool RepAfter { get; set; } 
        public byte House { get; set; }
        public NewNoteDTO[] Notes { get; set; }

        public NewBarDTO() { }

        public NewBarDTO(int partId, byte barNumber, bool repAfter, bool repBefore, byte house)
        {
            this.PartId = partId;
            this.RepBefore = repBefore;
            this.RepAfter = repAfter;
            this.BarNumber = barNumber;
            this.House = house;
        }
    }
}
