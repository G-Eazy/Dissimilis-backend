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

        /// <summary>
        /// Create new Bar using NewBarDTO
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BarDTO> CreateBar(NewBarDTO bar, uint userId)
        {
            if (bar is null) return null;

            Bar BarModel = new Bar(bar.BarNumber, bar.PartId);
            if (!ValidateUser(userId, BarModel.Part.Song)) return null;

            await this.context.Bars.AddAsync(BarModel);
            this.context.UserId = userId;
            await this.context.TrySaveChangesAsync();

            //Create a DTO object of the newly created Bar
            BarDTO BarModelDTO = new BarDTO() 
            { 
                Id = BarModel.Id, 
                PartId = BarModel.PartId, 
                BarNumber = BarModel.BarNumber 
            };

            return BarModelDTO;
        }

        /// <summary>
        /// Delete a bar by Id provided in BarDTO
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBarById(BarDTO bar, uint userId)
        {
            bool Deleted = false;
            Bar barModel = await FindBarById(bar.Id);

            //check if they are allowed to delete
            if (ValidateUser(userId, barModel.Part.Song))
            {
                this.context.Remove(barModel);
                Deleted = await this.context.TrySaveChangesAsync();
            }

            return Deleted;
        }

        /// <summary>
        /// Find bar by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Bar> FindBarById(int id)
        {
            if(id == 0)
            {
                throw new ArgumentException("The Id is not provided");
            }
            return await this.context.Bars.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Find or create a bar using BarDTO(if id empty, create new?)
        /// TODO fix it if this is not working
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="partId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BarDTO> FindOrCreateBar(BarDTO bar, uint userId)
        {
            Bar BarModel;
            if (bar is null) return null;
            
            if(bar.Id <= 0)
            {
                BarModel = new Bar(bar.BarNumber, bar.PartId);
                await this.context.Bars.AddAsync(BarModel);
                this.context.UserId = userId;
                await this.context.TrySaveChangesAsync();
            }
            else
            {
                BarModel = await FindBarById(bar.Id);
            }

            BarDTO NewBarDTO = new BarDTO() { BarNumber = BarModel.BarNumber, Id = bar.Id, PartId = bar.PartId };

            return NewBarDTO;
        }

        /// <summary>
        /// Update a bar by using its Id and BarDTO
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBarById(BarDTO bar, uint userId)
        {
            if (bar is null) return false;
            Bar BarModel = await this.context.Bars.SingleOrDefaultAsync(b => b.Id == bar.Id);
            if(!ValidateUser(userId, BarModel.Part.Song))
            if(bar.BarNumber != BarModel.BarNumber)
                BarModel.BarNumber = bar.BarNumber;

            this.context.UserId = userId;
            bool Updated = await this.context.TrySaveChangesAsync();
            
            return Updated;
        }

        /// <summary>
        /// Check if the user belongs to the bar it is trying to access/edit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool ValidateUser(uint userId, Song song)
        {
            try
            {
                if(userId == song.CreatedById)
                    return true;
                return false;
            }
            catch
            {
                throw new ArgumentException("The user is not allowed to edit on this song");
            }
        }
    }
}
