using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class BarDTO
    {
        public int Id { get; set; }
        public byte BarNumber { get; set; }
        public int PartId { get; set; }
        public bool RepBefore { get; set; }
        public bool RepAfter { get; set; }
        public byte House { get; set; }
        public Note[] Notes { get; set; }

        public BarDTO() { }

        public BarDTO(byte barnumber, bool repAfter, bool repBefore, byte house)
        {
            this.BarNumber = barnumber;
            this.RepAfter = repAfter;
            this.RepBefore = repBefore;
            this.House = house;
        }
    }
}
