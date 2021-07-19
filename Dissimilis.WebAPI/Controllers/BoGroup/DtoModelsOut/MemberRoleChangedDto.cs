using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut
{
    public class MemberRoleChangedDto
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public Role UpdatedRole { get; set; }
    }
}
