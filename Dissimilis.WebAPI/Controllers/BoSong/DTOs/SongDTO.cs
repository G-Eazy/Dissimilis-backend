using Dissimilis.WebAPI.Controllers.SuperDTOs;
using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.DTOs
{
    public class SongDTO : SuperDTO
    {

        public string Title { get; set; }
        
        public int ArrangerId { get; set; }
    
        public DateTime? UpdatedOn { get; set; }

        public SongDTO(Song s) : base(s.Id)
        {
            this.Title = s.Title;
            this.ArrangerId = s.ArrangerId;
            this.UpdatedOn = s.UpdatedOn;
        }
    }
}
