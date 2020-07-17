using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories
{
    class BarRepository : IBarRepository
    {
        private readonly DissimilisDbContext context;
        public BarRepository(DissimilisDbContext context)
        {
            this.context = context;
        }
        public async Task<BarDTO> CreateBar(NewBarDTO bar, int partId, uint userId)
        {
            Bar BarModel = new Bar(bar.BarNumber, partId);
            await this.context.Bars.AddAsync(BarModel);
            this.context.UserId = userId;
            await this.context.SaveChangesAsync();

            //Create a DTO object of the newly created Bar
            BarDTO BarModelDTO = new BarDTO() 
            { 
                Id = BarModel.Id, 
                PartId = BarModel.PartId, 
                BarNumber = BarModel.BarNumber 
            };

            return BarModelDTO;
        }

        public async Task<Bar> FindBarById(int id)
        {
            if(id == 0)
            {
                throw new Exception("The Id is not provided");
            }
            return await this.context.Bars.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Bar> FindBarByPriority(int priority)
        {
            return await this.context.Bars.SingleOrDefaultAsync(b => b.BarNumber == priority);
        }

        public async Task<BarDTO> FindOrCreateBar(BarDTO bar, int partId, uint userId)
        {
            Bar BarModel;
            if(bar.Id is 0)
            {
                BarModel = new Bar(bar.BarNumber, partId);
                await this.context.Bars.AddAsync(BarModel);
                this.context.UserId = userId;
                await this.context.SaveChangesAsync();
            }
            else
            {
                BarModel = await FindBarById(bar.Id);
            }

            BarDTO NewBarDTO = new BarDTO() { BarNumber = BarModel.BarNumber, Id = bar.Id, PartId = bar.PartId };

            return NewBarDTO;
        }

        public async Task<bool> UpdateBarById(BarDTO bar, uint userId)
        {
            bool Updated = false;
            Bar BarModel = await this.context.Bars.SingleOrDefaultAsync(b => b.Id == bar.Id);
            if(bar.BarNumber != BarModel.BarNumber)
            {
                BarModel.BarNumber = bar.BarNumber;
                Updated = true;
            }
            this.context.UserId = userId;
            await this.context.SaveChangesAsync();
            //TODO set true or false if updated worked
            return Updated;
        }
    }
}
