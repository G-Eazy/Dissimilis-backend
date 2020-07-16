using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewSongDTO
    {
        public string Title { get; set; }
        public int ArrangerId { get; set; }
        public string TimeSignature { get; set; }

    }
}
