using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using System.Linq;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut
{
    public class OrganisationByIdDto
    {
        public string OrganisationName { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public UserDto[] admins { get; set; }

        public OrganisationByIdDto(Organisation organisation)
        {
            OrganisationName = organisation.Name;
            Email = organisation.Email;
            Id = organisation.Id;
            Address = organisation.Address;
            PhoneNumber = organisation.PhoneNumber;
            Description = organisation.Description;
            admins = organisation.Users
                .Where(ou => ou.Role == Role.Admin)
                .Select(ou => new UserDto(ou.User))
                .ToArray();
        }
    }
}
