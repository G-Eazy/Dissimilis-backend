using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn
{
    public class GetGroupsQueryDto
    {
        public bool OnlyMyGroups { get; set; }

        public bool MyAdminGroups { get; set; }
        
        public int? OrganisationId { get; set; }
    }
}
