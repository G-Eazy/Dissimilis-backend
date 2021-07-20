using Dissimilis.DbContext.Models;
using System;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut
{
    public class GroupIndexDto
    {
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public string OrganisationName { get; set; }
        public int OrganisationId { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
    
    public GroupIndexDto(Group group)
        {
            GroupId = group.Id;
            GroupName = group.Name;
            OrganisationId = group.OrganisationId;
            OrganisationName = group.Organisation?.Name;
            CreatedOn = group.CreatedOn;
        }
    }
}
