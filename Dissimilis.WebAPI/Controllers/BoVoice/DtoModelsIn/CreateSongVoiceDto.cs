using System.ComponentModel.DataAnnotations;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn
{
    public class CreateSongVoiceDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Insturment { get; set; }

        public int VoiceNumber { get; set; }

    }
}
