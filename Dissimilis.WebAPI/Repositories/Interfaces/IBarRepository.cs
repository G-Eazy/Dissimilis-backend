using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    interface IBarRepository
    {
        Task<BarDTO> GetBar(int bar_it);
        Task<int> CreateBar(NewBarDTO bar, uint userId);
        Task<bool> UpdateBar(UpdateBarDTO bar, uint userId);
        Task<bool> DeleteBar(int barId, uint userId);

    }
}
