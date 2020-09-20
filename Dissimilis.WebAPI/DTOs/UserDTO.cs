using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Country Country { get; set; }
        public UserDTO(User u)
        {
            this.Id = u.Id;
            this.Name = u.Name;
            this.Email = u.Email;
            this.Country = u.Country;
        }
    }
}
