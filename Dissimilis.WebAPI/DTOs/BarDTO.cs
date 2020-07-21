using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class BarDTO : NewBarDTO
    {
        public int Id { get; set; }
        public NoteDTO[] Notes { get; set; }

        public BarDTO() { }

        public BarDTO(int id, int partId, byte barnumber, bool repAfter, bool repBefore, byte house)
        {
            this.Id = id;
            base.PartId = partId;
            base.BarNumber = barnumber;
            base.RepAfter = repAfter;
            base.RepBefore = repBefore;
            base.House = house;
        }
    }
}
