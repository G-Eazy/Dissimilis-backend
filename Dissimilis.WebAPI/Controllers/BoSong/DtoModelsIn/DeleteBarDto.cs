using System;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn
{
    public class DeleteBarDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int FromPosition { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DeleteLength { get; set; }
    }
}
