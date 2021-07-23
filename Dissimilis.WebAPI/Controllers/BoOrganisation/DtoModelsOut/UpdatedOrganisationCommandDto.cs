using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut
{
    public class UpdatedOrganisationCommandDto
    {
        public int OrganisationId { get; set; }

        public UpdatedOrganisationCommandDto(Organisation Organisation)
        {
            OrganisationId = Organisation.Id;
        }
    }
}