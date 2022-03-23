using Dissimilis.DbContext.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut
{
    public class UserRoleChangedDto
    {
        public int UserId { get; set; }
        
        public int OrganisationId { get; set; }
        public Role UpdatedRole { get; set; }
    }
}
