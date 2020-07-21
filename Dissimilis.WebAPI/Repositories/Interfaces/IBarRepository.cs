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
        Task<BarDTO> FindOrCreateBar(BarDTO bar, uint userId);
        Task<int> CreateBar(NewBarDTO bar, uint userId);
        Task<Bar> FindBarById(int id);
        Task<bool> UpdateBar(UpdateBarDTO bar, uint userId);
        Task<bool> DeleteBarById(int barId, uint userId);
        void UpdateBarNumbers(int barNumber, int partId, uint userId);

        Task<NoteDTO[]> FindAllNotesForBar(int barId);
    }
}
