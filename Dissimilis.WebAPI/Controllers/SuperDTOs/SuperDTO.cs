using Dissimilis.WebAPI.Database.Models;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.SuperDTOs
{
    public class SuperDTO
    {
        public int Id { get; set; }
        public SuperDTO() { }
        public SuperDTO(int Id) // TODO: generic?
        {
            this.Id = Id;
        }


    }
}
