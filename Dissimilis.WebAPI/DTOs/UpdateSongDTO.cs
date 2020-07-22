using Dissimilis.WebAPI.Database.Interfaces;
using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class UpdateSongDTO : NewSongDTO
    {
        public int Id { get; set; }
        
        public UpdateSongDTO() { }

        public UpdateSongDTO(ISong song) 
        {
            if (song is null)
                throw new ArgumentNullException(nameof(song));

            this.Id = song.Id;
            base.Title = song.Title;
            base.TimeSignature = song.TimeSignature;
        }
    }
}
