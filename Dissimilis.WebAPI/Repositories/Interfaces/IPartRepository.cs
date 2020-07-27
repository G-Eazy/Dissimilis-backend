﻿using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    interface IPartRepository
    {
        Task<PartDTO> GetPart(int partId);
        Task<int> CreatePart(NewPartDTO NewPartObject, uint userId);
        Task<Instrument> CreateOrFindInstrument(string InstrumentName, uint userId);
        Task<bool> UpdatePart(UpdatePartDTO UpdatePartObject, uint userId);
        Task<bool> DeletePart(int partId, uint userId);
    }
}