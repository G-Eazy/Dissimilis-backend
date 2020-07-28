﻿using Dissimilis.WebAPI.Database;
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
            BarModelObject.ChordsAndNotes = await FindAllNotesForBar(BarModel.Id);
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
            if (!IsValidDTO<NewBarDTO, NewBarDTOValidator>(NewBarObject)) return 0;

            Bar CheckBarNumber = await this.context.Bars
                .SingleOrDefaultAsync(b => b.BarNumber == NewBarObject.BarNumber 
                                        && b.PartId == NewBarObject.PartId);
            if (CheckBarNumber != null)
                await UpdateBarNumbers(CheckBarNumber.BarNumber, CheckBarNumber.PartId, userId);

            Bar BarModel = new Bar(NewBarObject);

            await this.context.AddAsync(BarModel);
            this.context.UserId = userId;
            if(await this.context.TrySaveChangesAsync()) return BarModel.Id;
            else return 0;
        }
        /// <summary>
        /// Creates new [empty] bars for all parts of a song, at the same position.
        /// </summary>
        /// <param name="NewBarObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BarDTO[]> CreateBarHelper(NewBarDTO NewBarObject, uint userId)
        {
            // Saving all created bars' Id  
            List<Bar> bars = new List<Bar>();

            if (!IsValidDTO<NewBarDTO, NewBarDTOValidator>(NewBarObject)) return null;
            
            // Get all parts of this song
            Part[] parts = await this.context.Parts
                .Include(p => p.Song)
                .Where(p => p.SongId == p.Song.Id)
                .OrderBy(p => p.PartNumber)
                .ToArrayAsync();
            
           
            // Creating bars for all parts in this song
            foreach (Part part in parts)
            { 
                Bar CheckBarNumber = await this.context.Bars
                    .SingleOrDefaultAsync(b => b.BarNumber == NewBarObject.BarNumber 
                                            && b.PartId == part.Id);
                if (CheckBarNumber != null)
                    await UpdateBarNumbers(CheckBarNumber.BarNumber, part.Id, userId);

                Bar BarModel = new Bar(NewBarObject);
                BarModel.PartId = part.Id;
                await this.context.AddAsync(BarModel);
                bars.Add(BarModel);
            
            }
            
            this.context.UserId = userId;
            await this.context.TrySaveChangesAsync();

            return bars.Select(b => new BarDTO(b)).ToArray();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partId"></param>
        /// <param name="barObjects"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CreateAllBars(int partId, NewBarDTO[] barObjects, uint userId)
        {
            //return true if the barobject is empty because that just means parts dont have any bars
            if (barObjects == null || barObjects.Count() == 0) return true;
            if (partId is 0) return false;

            ushort barNumber = 1;

            foreach (NewBarDTO bar in barObjects)
            {
                bar.PartId = partId;
                bar.BarNumber = barNumber++;
                int barId = await CreateBar(bar, userId);
                bool notesCreated = await this.noteRepository.CreateAllNotes(barId, bar.ChordsAndNotes, userId);
                if (!notesCreated) return false;
            }

            return true;
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

        /// <summary>
        /// Get all the notes associated with this bar
        /// </summary>
        /// <param name="barId"></param>
        /// <returns></returns>
        private async Task<NoteDTO[]> FindAllNotesForBar(int barId)
        {
            var AllNotes = this.context.Notes
                           .Where(n => n.BarId == barId)
                           .OrderBy(n => n.NoteNumber)
                           .Select(n => new NoteDTO(n))
                           .ToArray();

            return AllNotes;
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
