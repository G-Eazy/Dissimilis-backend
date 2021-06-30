using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn
{
    public class DuplicateVoiceDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
