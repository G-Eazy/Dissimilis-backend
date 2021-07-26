using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn
{
    public class UpdateGroupAndOrganisationDto
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }
    }
}