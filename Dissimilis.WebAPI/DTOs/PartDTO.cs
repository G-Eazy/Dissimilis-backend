using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class PartDTO : NewPartDTO
    {
        public int Id { get; set; }
        public BarDTO[] Bars { get; set; }

        public PartDTO() { }

        public PartDTO(Part existingPart)
        {
            this.Id = existingPart.Id;
            this.PartNumber = existingPart.PartNumber;
            this.SongId = existingPart.SongId;
            this.Title = existingPart.Instrument.Name;
        }

        public PartDTO(int id, byte partnumber, int songid, string InstrumentName)
        {
            this.Id = id;
            this.PartNumber = partnumber;
            this.SongId = songid;
            this.Title = InstrumentName;
        }
    }
}