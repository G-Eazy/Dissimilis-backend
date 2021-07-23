using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn
{
    public class AddMemberDto
    {
        public int NewMemberUserId { get; set; }
        public Role NewMemberRole { get; set; }
    }
}
