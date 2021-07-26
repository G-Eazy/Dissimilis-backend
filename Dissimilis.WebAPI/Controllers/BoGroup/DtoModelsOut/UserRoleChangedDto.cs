using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut
{
    public class UserRoleChangedDto
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public Role UpdatedRole { get; set; }
    }
}
