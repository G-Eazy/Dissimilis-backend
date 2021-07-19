using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut
{
    public class OrganisationByIdDto
    {
        public string Name { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int GroupId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public UserDto[] admins { get; set; }

        public OrganisationByIdDto(Group group)
        {
            Name = group.Name;
            OrganisationId = group.OrganisationId;
            OrganisationName = group.Organisation.Name;
            GroupId = group.Id;
            Address = group.Address;
            PhoneNumber = group.PhoneNumber;
            Description = group.Description;
            admins = group.Users
                .Where(gu => gu.Role == Role.Admin)
                .Select(gu => new UserDto(gu.User))
                .ToArray();
        }
    }
}
