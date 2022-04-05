using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
public class ShortGroupDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }

    public ShortGroupDto(Group group)
    {
        GroupId = group.Id;
        GroupName = group.Name;
    }
}
}