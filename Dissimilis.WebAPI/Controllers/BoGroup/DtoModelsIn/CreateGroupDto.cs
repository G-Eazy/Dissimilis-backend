using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn
{
    public class CreateGroupDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public int OrganisationId { get; set; }
        [Required]
        public int FirstAdminId { get; set; }
    }
}
