using Dissimilis.DbContext.Models;
using System;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut
{
    public class GroupIndexDto
    {
        public string OrganisationName { get; set; }

        public string GroupName { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }
    
    public GroupIndexDto(Group group)
        {
            OrganisationName = group.Organisation?.Name;
            GroupName = group.Name;
            CreatedOn = group.CreatedOn;
        }
    }
}
