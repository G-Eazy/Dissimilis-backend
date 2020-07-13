using Dissimilis.WebAPI.Database.Models;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoUser.DTOs
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
