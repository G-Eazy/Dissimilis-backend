using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut
{
    public class UserOrganisationUpdatedDto
    {
        public int UserId { get; set; }
        public int OrganisationId { get; set; }
        public string Role { get; set; }

        public UserOrganisationAddedDto(int userId, int organisationId, string role)
        {
            UserId = userId;
            OrganisationId = organisationId;
            Role = role;
        }
    }
}
