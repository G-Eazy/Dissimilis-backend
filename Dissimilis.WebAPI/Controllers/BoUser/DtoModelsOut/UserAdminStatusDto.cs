using Dissimilis.DbContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut
{
    public class UserAdminStatusDto
    {
        public bool SystemAdmin { get; set; }
        public bool OrganisationAdmin { get; set; }
        public bool GroupAdmin { get; set; }
    }
}
