using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class SongDTO 
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string ArrangerName { get; set; }
    
        public DateTime? UpdatedOn { get; set; }

        public PartDTO[] Parts { get; set; }

        public SongDTO(Song song)
        {
            this.Id = song.Id;
            this.Title = song.Title;
            this.ArrangerName = song.Arranger.Name;
            this.UpdatedOn = song.UpdatedOn;
        }
    }
}
