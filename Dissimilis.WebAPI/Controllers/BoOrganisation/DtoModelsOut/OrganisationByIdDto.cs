using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using System.Linq;

namespace Dissimilis.WebAPI.Controllers.Boorganisation.DtoModelsOut
{
    public class OrganisationByIdDto
    {
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public UserDto[] admins { get; set; }

        public OrganisationByIdDto(Organisation organisation)
        {
            Name = organisation.Name;
            OrganisationName = organisation.Name;
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
