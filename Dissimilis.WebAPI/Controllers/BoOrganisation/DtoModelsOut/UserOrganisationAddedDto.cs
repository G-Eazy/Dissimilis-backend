using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut
{
    public class UserOrganisationAddedDto
    {
        public int UserId { get; set; }
        public int OrganisationId { get; set; }
        public string Role { get; set; }
    }
}
