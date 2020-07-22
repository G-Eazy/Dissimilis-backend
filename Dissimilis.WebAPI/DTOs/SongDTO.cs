using Dissimilis.WebAPI.Database.Interfaces;
using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class SongDTO : NewSongDTO
    {
        public int Id { get; set; } 
        
        public string ArrangerName { get; set; }
    
        public DateTime? UpdatedOn { get; set; }

        public PartDTO[] Parts { get; set; }

        public SongDTO(ISong song)
        {
            if (song != null && song.Arranger != null)
            { 
                this.Id = song.Id;
                base.Title = song.Title;
                this.ArrangerName = song.Arranger.Name;
                this.UpdatedOn = song.UpdatedOn;
                base.TimeSignature = song.TimeSignature;
            }
            else
                throw new ArgumentNullException(nameof(song));

        }
    }
}
