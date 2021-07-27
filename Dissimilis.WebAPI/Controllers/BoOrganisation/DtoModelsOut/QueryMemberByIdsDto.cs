using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut
{
    public class QueryMemberByIdsDto
    {
        public int OrganisationId { get; set; }
        public int UserId { get; set; }
    }
}
