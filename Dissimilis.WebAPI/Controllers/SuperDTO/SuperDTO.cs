using Dissimilis.WebAPI.Database.Models;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.SuperDTO
{
    public class SuperDTO
    {
        public int Id { get; set; }
        public SuperDTO(User u)
        {
            this.Id = u.Id;
        }


    }
}
