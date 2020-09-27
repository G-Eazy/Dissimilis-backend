using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn
{
    public class UpdatedBarCommandDto
    {
        public int BarId { get; set; }

        public UpdatedBarCommandDto(SongBar songBar)
        {
            BarId = songBar.Id;
        }
    }
}
