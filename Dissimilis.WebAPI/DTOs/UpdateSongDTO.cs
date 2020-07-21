using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class UpdateSongDTO : NewSongDTO
    {
        public int Id { get; set; }
        
        public UpdateSongDTO() { }

        public UpdateSongDTO(Song s) 
        {
            this.Id = s.Id;
            base.Title = s.Title;
            base.TimeSignature = s.TimeSignature;
        }
    }
}
