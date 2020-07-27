using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewPartDTO 
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public ushort PartNumber { get; set; }
        public NewBarDTO[] Bars { get; set; }

        public NewPartDTO() { }
    } 
}
