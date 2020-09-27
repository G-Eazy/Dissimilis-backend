using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut
{
    public class BarDto : BoSong.DtoModelsOut.BarDto
    {
        public BarDto(SongBar songBar) : base(songBar) { }
    }
}
