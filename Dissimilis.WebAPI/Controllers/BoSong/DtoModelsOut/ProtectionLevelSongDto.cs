using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Song;

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
