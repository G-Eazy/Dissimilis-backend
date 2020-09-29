using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public UserDto(User user)
        {
            UserId = user.Id;
            Name = user.Name;
            Email = user.Email;
        }
    }
}
