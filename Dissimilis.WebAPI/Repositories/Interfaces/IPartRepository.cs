using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    interface IPartRepository
    {
        Task<PartDTO> GetPartById(int partId);
        Task<int> CreatePart(NewPartDTO NewPartObject, uint userId);
        void UpdatePartNumbers(int partNumber, int songId, uint userId);
        Task<Instrument> CreateOrFindInstrument(string InstrumentName, uint userId);
        Task<BarDTO[]> GetAllBarsForParts(int partId);
        Task<bool> UpdatePart(UpdatePartDTO UpdatePartObject, uint userId);
        Task<bool> DeletePart(int partId, uint userId);
    }
}
