﻿using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    interface IBarRepository
    {
        Task<BarDTO> FindOrCreateBar(BarDTO bar, uint userId);
        Task<BarDTO> CreateBar(NewBarDTO bar, uint userId);
        Task<Bar> FindBarById(int id);
        Task<bool> UpdateBarById(BarDTO bar, uint userId);
        Task<bool> DeleteBarById(BarDTO bar, uint userId);
        bool ValidateUser(uint userId, Song bar);
    }
}