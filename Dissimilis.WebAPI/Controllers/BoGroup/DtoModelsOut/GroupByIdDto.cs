using Dissimilis.DbContext.Models;
using System;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut
{
    public class GroupByIdDto
    {
        public string OrganisationName { get; set; }

        public string GroupName { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }


        public GroupByIdDto(Group group)
        {
            OrganisationName = group.Organisation?.Name;
            GroupName = group.Name;
            CreatedOn = group.CreatedOn;
        }
    }
}
