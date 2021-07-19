using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn
{
    public class ChangeUserRoleDto
    {
        public int MemberId { get; set; }
        public Role RoleToSet { get; set; }
    }
}
