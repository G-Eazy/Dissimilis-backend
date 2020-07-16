using Dissimilis.WebAPI.Database.Models;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class SuperDTO
    {
        public int Id { get; set; }

        // Temporary
        public int Id2 { get; set; }
        public SuperDTO() { }
        public SuperDTO(int Id) 
        {
            this.Id = Id;
        }


    }
}
