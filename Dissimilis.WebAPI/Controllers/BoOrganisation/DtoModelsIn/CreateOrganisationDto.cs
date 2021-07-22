using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsIn
{
    public class CreateOrganisationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]    
        public int FirstAdminId { get; set; }
    }
}
