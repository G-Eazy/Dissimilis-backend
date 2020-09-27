﻿using Dissimilis.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;

namespace Dissimilis.WebAPI.Repositories
{
    public class PartRepository : BaseRepository, IPartRepository
    {
        private DissimilisDbContext context;
        private BarRepository barRepository;
        private NoteRepository noteRepository;

        public PartRepository(DissimilisDbContext context)
        {
            this.context = context;
            this.barRepository = new BarRepository(context);
            this.noteRepository = new NoteRepository(context);
        }

        /// <summary>
        /// Get part by id 
        /// </summary>
        /// <param name="partId"></param>
        /// <returns>PartDTO</returns>
        public async Task<PartDTO> GetPart(int partId)
        {
            if (partId <= 0) return null;

            SongVoice existsSongVoice = await this.context.SongParts
                .Include(p => p.Instrument)
                .SingleOrDefaultAsync(p => p.Id == partId);

            PartDTO PartObject = null;

            if (existsSongVoice != null)
            {
                PartObject = new PartDTO(existsSongVoice);
                PartObject.Bars = await GetAllBarsForParts(existsSongVoice.Id);
            }

            return PartObject;
        }


        /// <summary>
        /// Create a new Part to a Song
        /// </summary>
        /// <param name="createPartObject"></param>
        /// <param name="userId"></param>
        /// <returns>SuperDTO</returns>
        public async Task<int> CreatePart(CreatePartDto createPartObject, uint userId)
        {
            //Check if values are present in DTO, return 0 if one is missing
            if (!IsValidDTO<CreatePartDto, NewPartDTOValidator>(createPartObject)) return 0;

            var ExistsSong = await this.context.Songs
                .SingleOrDefaultAsync(s => s.Id == createPartObject.SongId);

            if (!ValidateUser(userId, ExistsSong)) return 0;

            var ExistsInstrument = await CreateOrFindInstrument(createPartObject.Title, userId);
            // This will trigger BadRequest from controller, Will remove when we have exception handler
            if (ExistsInstrument == null)
                return 0;

            int result = 0;

            if (ExistsSong != null)
            {
                var PartModelObject = new SongVoice(ExistsSong.Id, ExistsInstrument.Id, createPartObject.PartNumber);
                
                await this.context.SongParts.AddAsync(PartModelObject);
                result = PartModelObject.Id;
            }
            return result;
        }

        /// <summary>
        /// Create all parts in a NewPartDTO array
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="PartObjects"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CreateAllParts(int songId, CreatePartDto[] PartObjects, uint userId)
        {
            if (PartObjects == null || PartObjects.Count() == 0) return false;
            if (songId <= 0) return false;

            byte partNumber = 1;

            foreach(CreatePartDto part in PartObjects)
            {
                part.SongId = songId;
                part.PartNumber = partNumber++;
                await CreatePart(part, userId);
            }

            await this.context.SaveChangesAsync();

            int[] allParts = await this.context.SongParts
                .Where(b => b.SongId== songId)
                .OrderBy(b => b.PartNumber)
                .Select(b => b.Id)
                .ToArrayAsync();

            for (int i = 0; i < allParts.Count(); i++)
            {
                await this.barRepository.CreateAllBars(allParts[i], PartObjects[i].Bars, userId);
            }

            await this.context.SaveChangesAsync();


            for (int i = 0; i < allParts.Count(); i++)
            {
                int[] allBars = await this.context.SongBars
                   .Where(b => b.PartId == allParts[i])
                   .OrderBy(b => b.BarNumber)
                   .Select(b => b.Id)
                   .ToArrayAsync();

                for (int j = 0; j < allBars.Count(); j++)
                {
                    await this.noteRepository.CreateAllNotes(allBars[j], PartObjects[i].Bars[j].ChordsAndNotes, userId);
                }
            }

            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteParts(int songId, uint userId)
        {
            var AllParts = this.context.SongParts.Where(p => p.SongId == songId);
            foreach (SongVoice part in AllParts)
            {
                this.context.SongParts.Remove(part);
            }

            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Update the Part Numbers
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="songId"></param>
        /// <param name="userId"></param>
        private async Task<bool> UpdatePartNumbers(int partNumber, int songId, uint userId)
        {
            SongVoice[] AllParts = this.context.SongParts.Where(b => b.SongId == songId)
                .OrderBy(x => x.PartNumber)
                .ToArray();

            for (int i = partNumber - 1; i < AllParts.Count(); i++)
            {
                AllParts[i].PartNumber++;
            }

            await this.context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Looks for an instrument with title InstrumentName, and creates if non-existant
        /// </summary>
        /// <param name="InstrumentName"></param>
        /// <param name="userId"></param>
        /// <returns>(Model) Instrument</returns>
        public async Task<Instrument> CreateOrFindInstrument(string InstrumentName, uint userId)
        {
            if (String.IsNullOrWhiteSpace(InstrumentName))
                //throw new ArgumentNullException(nameof(InstrumentName)); // Commenting out until we have exception handler
                return null;

            var ExistsInstrument = await this.context.Instruments
                .SingleOrDefaultAsync(i => i.Name == InstrumentName);
            if (ExistsInstrument is null)
            {
                ExistsInstrument = new Instrument(InstrumentName);
                await this.context.Instruments
                    .AddAsync(ExistsInstrument);
                
                await this.context.SaveChangesAsync();
            }
            return ExistsInstrument;
        }

        /// <summary>
        /// UpdatePart using UpdatePartDTO
        /// </summary>
        /// <param name="UpdatePartObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePart(UpdatePartDTO UpdatePartObject, uint userId)
        {
            bool Updated = false;
            if (! IsValidDTO<UpdatePartDTO, UpdatePartDTOValidator>(UpdatePartObject)) return Updated;

            var PartModelObject = await this.context.SongParts
                .Include(p => p.Song)
                .Include(p => p.Instrument)
                .SingleOrDefaultAsync(s => s.Id == UpdatePartObject.Id);

            if (PartModelObject != null && ValidateUser(userId, PartModelObject.Song))
            {
                SongVoice checkSongVoiceNumber = await this.context.SongParts.SingleOrDefaultAsync(p => p.PartNumber == UpdatePartObject.PartNumber && p.SongId == UpdatePartObject.SongId);
                if (checkSongVoiceNumber != null)
                {
                    await UpdatePartNumbers(UpdatePartObject.PartNumber, UpdatePartObject.SongId, userId);
                }
                // Checking for differences between Model and DTO 
                if (PartModelObject.Instrument.Name != UpdatePartObject.Title)
                {
                    var NewInstrument = await CreateOrFindInstrument(UpdatePartObject.Title, userId);
                    PartModelObject.InstrumentId = NewInstrument.Id;
                }
                if (PartModelObject.PartNumber != UpdatePartObject.PartNumber)
                {
                    PartModelObject.PartNumber = UpdatePartObject.PartNumber;
                }

                
                await this.context.SaveChangesAsync();
            }
            return Updated;
        }


        /// <summary>
        /// Delete Part by partId
        /// </summary>
        /// <param name="partId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePart(int partId, uint userId)
        {
            bool Deleted = false;
            if (partId <= 0) return Deleted;

            var PartModelObject = await this.context.SongParts
                .Include(p => p.Song)
                .SingleOrDefaultAsync(p => p.Id == partId);

            if (PartModelObject != null && ValidateUser(userId, PartModelObject.Song))
            {
                this.context.SongParts.Remove(PartModelObject);
                 await this.context.SaveChangesAsync();
            }
            
            return Deleted;
        }


        /// <summary>
        /// Get all associated bars with this part
        /// </summary>
        /// <param name="partId"></param>
        /// <returns>BarDTOArray</returns>
        private async Task<BarDto[]> GetAllBarsForParts(int partId)
        {
            BarDto[] AllBars = await this.context.SongBars
                .Where(b => b.PartId == partId)
                .OrderBy(b => b.BarNumber)
                .Select(b => new BarDto(b))
                .ToArrayAsync();

            var BarIds = AllBars.Select(x => x.BarId);

            var AllNotes =  await this.context.SongNotes
                .Where(n => BarIds.Contains(n.BarId))
                .OrderBy(n => n.NoteNumber)
                .Select(n => new NoteDto(n))
                .ToArrayAsync();

            foreach (var bar in AllBars)
            {
                bar.ChordsAndNotes = AllNotes.Where(x => x.BarId == bar.BarId).ToArray();
            }

            return AllBars;
        }
    }
}
