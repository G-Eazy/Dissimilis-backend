using Dissimilis.DbContext.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn
{
    public class UpdateProtectionLevelDto
    {
        [Required]
        public string ProtectionLevel { get; set; }

    }
}
