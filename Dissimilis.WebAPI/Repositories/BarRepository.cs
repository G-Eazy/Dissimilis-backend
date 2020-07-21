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
    public class BarRepository : IBarRepository
    {
        private readonly DissimilisDbContext context;
        private readonly NoteRepository noteRepository;

        public BarRepository(DissimilisDbContext context)
        {
            this.noteRepository = new NoteRepository(context);
            this.context = context;
        }

        /// <summary>
        /// Get a bar with all notes associated with it
        /// </summary>
        /// <param name="bar_id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BarDTO> GetBar(int bar_id)
        {
            if (bar_id is 0) return null;
            Bar BarModel = await this.context.Bars.SingleOrDefaultAsync(x => x.Id == bar_id);
            if (BarModel is null) 
                return null;

            BarDTO BarModelObject = new BarDTO(BarModel.Id, BarModel.PartId, BarModel.BarNumber, BarModel.RepAfter, BarModel.RepBefore, BarModel.House);
            BarModelObject.Notes = await FindAllNotesForBar(BarModel.Id);
            return BarModelObject;
        }

        /// <summary>
        /// Create new Bar using NewBarDTO
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BarDTO> CreateBar(NewBarDTO bar, uint userId)
        {
            if (bar is null) 
                return null;

            Bar CheckBarNumber = await this.context.Bars.SingleOrDefaultAsync(b => b.BarNumber == bar.BarNumber && b.PartId == bar.PartId);
            if (CheckBarNumber != null)
            {
                UpdateBarNumbers(CheckBarNumber.BarNumber, CheckBarNumber.PartId, userId);
            }

            Part part = await this.context.Parts
                .Include(x => x.Song).SingleOrDefaultAsync(x => x.Id == bar.PartId);
            
            if (!ValidateUser(userId, part.Song)) 
                return null;

            Bar BarModel = new Bar(bar.BarNumber, bar.PartId);
            this.context.UserId = userId;
            await this.context.AddAsync(BarModel);
            await this.context.TrySaveChangesAsync();

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

        public async void UpdateBarNumbers(int barNumber, int partId, uint userId)
        {
            Bar[] allbars = this.context.Bars.Where(b => b.PartId == partId)
                .OrderBy(x => x.BarNumber)
                .ToArray();

            for(int i = barNumber-1; i < allbars.Count(); i++)
            {
                allbars[i].BarNumber += 1;
            }

            this.context.UserId = userId;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete a bar by Id provided in BarDTO
        /// </summary>
        /// <param name="bar_id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBarById(int bar_id, uint userId)
        {
            Bar barModel = await FindBarById(bar_id);
            if (!ValidateUser(userId, barModel.Part.Song)) 
                return false;
               
            this.context.Remove(barModel);
            bool Deleted = await context.TrySaveChangesAsync();

            return Deleted;
        }

        public async Task<NoteDTO[]> FindAllNotesForBar(int barId)
        {
            Note[] allNotes = this.context.Notes.Where(x => x.BarId == barId).ToArray();
            NoteDTO[] NoteDTOArray = new NoteDTO[allNotes.Length];

            for (int i = 0; i < allNotes.Length; i++)
            {
                NoteDTOArray[i] = await this.noteRepository.GetNote(allNotes[i].Id);
            }

            return NoteDTOArray;
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

            BarDTO NewBarDTO = new BarDTO(BarModel.Id, BarModel.PartId, BarModel.BarNumber, BarModel.RepAfter, BarModel.RepBefore, BarModel.House);
            NewBarDTO.Notes = await FindAllNotesForBar(NewBarDTO.Id); 

            return NewBarDTO;
        }

        /// <summary>
        /// Update a bar by using its Id and BarDTO
        /// </summary>
        /// <param name="barObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBar(UpdateBarDTO barObject, uint userId)
        {
            if (barObject is null) return false;

            Bar BarModel = await this.context.Bars
                .Include(x => x.Part)
                .ThenInclude(x => x.Song)
                .SingleOrDefaultAsync(b => b.Id == barObject.Id);
            if(!ValidateUser(userId, BarModel.Part.Song))
            
            if(barObject.BarNumber != BarModel.BarNumber) BarModel.BarNumber = barObject.BarNumber;
            if (barObject.RepAfter != BarModel.RepAfter) BarModel.RepAfter = barObject.RepAfter;
            if (barObject.RepBefore != BarModel.RepBefore) BarModel.RepBefore = barObject.RepBefore;
            if (barObject.House != BarModel.House) BarModel.House = barObject.House;

            this.context.UserId = userId;
            bool Updated = await this.context.TrySaveChangesAsync();
            
            return Updated;
        }

        /// <summary>
        /// Check if the user belongs to the song it is trying to access/edit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="song"></param>
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
