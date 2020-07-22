using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewPartDTO : IDTO
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public byte PartNumber { get; set; }
    } 
}
