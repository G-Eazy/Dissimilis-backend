using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn
{
    public class UpdatedSongCommandDto
    {
        public int SongId { get; set; }

        public UpdatedSongCommandDto(Song song)
        {
            SongId = song.Id;
        }
    }
}
