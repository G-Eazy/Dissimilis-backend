using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class PartDTO
    {
        public int Id { get; set; }
        public byte PartNumber { set; get; }
        public int SongId { get; set; }
        public int InstrumentId { get; set; }

        public PartDTO() { }

        public PartDTO(int id, byte partnumber, int songid, int instrumentid)
        {
            this.Id = id;
            this.PartNumber = partnumber;
            this.SongId = songid;
            this.InstrumentId = instrumentid;
        }
    }
}
