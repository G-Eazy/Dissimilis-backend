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
    }
}
