using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn
{
    public class CreateSongVoiceDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Instrument { get; set; }

        public int VoiceNumber { get; set; }

    }
}
