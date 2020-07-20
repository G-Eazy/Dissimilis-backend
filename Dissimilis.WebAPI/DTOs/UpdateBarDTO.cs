using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class UpdateBarDTO
    {
        public int Id { get; set; }
        public byte BarNumber { get; set; }
        public bool RepBefore { get; set; }
        public bool RepAfter { get; set; }
        public byte House { get; set; }
    }
}
