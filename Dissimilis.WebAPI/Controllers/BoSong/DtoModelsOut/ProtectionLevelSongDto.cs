using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class ProtectionLevelSongDto
    {
        public int SongId { get; set; }
        public string ProtectionLevel { get; set; }
        
        public ProtectionLevelSongDto(Song song)
        {
            ProtectionLevel = song.ProtectionLevel.GetDescription();
            SongId = song.Id;
        }
    }


}
