using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut
{
    public class UserUpdatedDto
    {
        public int UserId { get; set; }

        public UserUpdatedDto(int userId)
        {
            UserId = userId;
        }
    }
}
