using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Repositories.Interfaces;
using Dissimilis.WebAPI.Repositories.Validators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories
{
    public class BarRepository : BaseRepository, IBarRepository
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
        /// <returns></returns>
        public async Task<BarDTO> GetBar(int bar_id)
        {
            if (bar_id <= 0) return null;

            Bar BarModel = await this.context.Bars.SingleOrDefaultAsync(x => x.Id == bar_id);
            if (BarModel is null) 
                return null;

            BarDTO BarModelObject = new BarDTO(BarModel);
            BarModelObject.Notes = await FindAllNotesForBar(BarModel.Id);
            return BarModelObject;
        }

        /// <summary>
        /// Create new Bar using NewBarDTO
        /// </summary>
        /// <param name="NewBarObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CreateBar(NewBarDTO NewBarObject, uint userId)
        {
            if (! IsValidDTO<NewBarDTO, NewBarDTOValidator>(NewBarObject)) return 0;

            Bar CheckBarNumber = await this.context.Bars
                .SingleOrDefaultAsync(b => b.BarNumber == NewBarObject.BarNumber 
                                        && b.PartId == NewBarObject.PartId);
            if (CheckBarNumber != null)
                await UpdateBarNumbers(CheckBarNumber.BarNumber, CheckBarNumber.PartId, userId);

            Bar BarModel = new Bar(NewBarObject.BarNumber, NewBarObject.PartId);

            await this.context.AddAsync(BarModel);
            this.context.UserId = userId;
            await this.context.TrySaveChangesAsync();

            return BarModel.Id;
        }

        /// <summary>
        /// Update Bar numbers
        /// </summary>
        /// <param name="barNumber"></param>
        /// <param name="partId"></param>
        /// <param name="userId"></param>
        private async Task<bool> UpdateBarNumbers(int barNumber, int partId, uint userId)
        {
            Bar[] allbars = this.context.Bars.Where(b => b.PartId == partId)
                .OrderBy(x => x.BarNumber)
                .ToArray();

            for(int i = barNumber-1; i < allbars.Count(); i++)
            {
                allbars[i].BarNumber++;
            }

            this.context.UserId = userId;
            await this.context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Delete a bar by Id provided in BarDTO
        /// </summary>
        /// <param name="barId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBar(int barId, uint userId)
        {
            bool Deleted = false;
            if (barId <= 0) return Deleted;

            Bar barModel = await this.context.Bars
                .Include(b => b.Part)
                .ThenInclude(b => b.Song)
                .SingleOrDefaultAsync(x => x.Id == barId);
            if (barModel != null &&  ValidateUser(userId, barModel.Part.Song))
            {
                this.context.Remove(barModel);
                Deleted = await context.TrySaveChangesAsync();
            }

            return Deleted;
        }

        private async Task<NoteDTO[]> FindAllNotesForBar(int barId)
        {
            int[] allNotes = this.context.Notes
                .Where(x => x.BarId == barId)
                .OrderBy(x => x.NoteNumber)
                .Select(x => x.Id)
                .ToArray();

            NoteDTO[] NoteDTOArray = new NoteDTO[allNotes.Count()];

            for (int i = 0; i < allNotes.Count(); i++)
            {
                NoteDTOArray[i] = await this.noteRepository.GetNote(allNotes[i]);
            }

            return NoteDTOArray;
        }

        /// <summary>
        /// Update a bar using UpdateBarDTO
        /// </summary>
        /// <param name="UpdateBarObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBar(UpdateBarDTO UpdateBarObject, uint userId)
        {
            bool Updated = false;

            if (! IsValidDTO<UpdateBarDTO, UpdateBarDTOValidator>(UpdateBarObject)) return Updated;
            
            Bar BarModel = await this.context.Bars
                .Include(x => x.Part)
                .ThenInclude(x => x.Song)
                .SingleOrDefaultAsync(b => b.Id == UpdateBarObject.Id);

            if (BarModel != null && ValidateUser(userId, BarModel.Part.Song))
            {
                Bar CheckBarNumber = await this.context.Bars.SingleOrDefaultAsync(b => b.BarNumber == UpdateBarObject.BarNumber && b.PartId == UpdateBarObject.PartId);
                if (CheckBarNumber != null)
                {
                    await UpdateBarNumbers(UpdateBarObject.BarNumber, UpdateBarObject.PartId, userId);
                }

                if (UpdateBarObject.BarNumber != BarModel.BarNumber) BarModel.BarNumber = UpdateBarObject.BarNumber;
                if (UpdateBarObject.RepAfter != BarModel.RepAfter) BarModel.RepAfter = UpdateBarObject.RepAfter;
                if (UpdateBarObject.RepBefore != BarModel.RepBefore) BarModel.RepBefore = UpdateBarObject.RepBefore;
                if (UpdateBarObject.House != BarModel.House) BarModel.House = UpdateBarObject.House;

                this.context.UserId = userId;
                Updated = await this.context.TrySaveChangesAsync();
            }

            return Updated;
        }
    }
}
