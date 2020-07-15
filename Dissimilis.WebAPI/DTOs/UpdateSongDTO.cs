using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class UpdateSongDTO : SuperDTO
    {
        // Useful properties for updating go here

        public UpdateSongDTO(int Id) : base(Id)
        { }
    }
}
