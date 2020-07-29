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

        public PartDTO[] Voices { get; set; }

        public SongDTO(ISong song)
        {

            if (song != null)
            { 
                this.Id = song.Id;
                base.Title = song.Title;
                if (song.Arranger != null)
                    this.ArrangerName = song.Arranger.Name;
                else
                    this.ArrangerName = "Unknown";
                this.UpdatedOn = song.UpdatedOn;
                base.TimeSignature = song.TimeSignature;
            }
            else
                throw new ArgumentNullException(nameof(song));

        }
    }
}
