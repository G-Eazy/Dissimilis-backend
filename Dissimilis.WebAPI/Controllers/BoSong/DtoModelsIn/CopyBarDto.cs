using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn
{
    public class CopyBarDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int FromPosition { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CopyLength { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ToPosition { get; set; }
    }
}
