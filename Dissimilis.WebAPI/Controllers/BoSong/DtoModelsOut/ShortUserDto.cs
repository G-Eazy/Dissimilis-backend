using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
public class ShortUserDto
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public ShortUserDto(User user)
    {
        UserId = user.Id;
        Name = user.Name;
        Email = user.Email;
    }
}
}