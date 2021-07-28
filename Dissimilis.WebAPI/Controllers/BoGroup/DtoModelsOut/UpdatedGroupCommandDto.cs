using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut
{
    public class UpdatedGroupCommandDto
    {
        public int GroupId { get; set; }

        public UpdatedGroupCommandDto(Group Group)
        {
            GroupId = Group.Id;
        }
    }
}