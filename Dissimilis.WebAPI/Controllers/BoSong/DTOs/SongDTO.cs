using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.DTOs
{
    public class SongDTO
    {
        public string Title { get; set; }
        
        public int ArrangerId { get; set; }

        public SongDTO(Song s)
        {
            this.Title = s.Title;
            this.ArrangerId = s.ArrangerId;
        }
    }
}
