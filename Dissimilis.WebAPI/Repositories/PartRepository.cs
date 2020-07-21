using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Repositories.Interfaces;

namespace Dissimilis.WebAPI.Repositories
{
    public class PartRepository : BaseRepository, IPartRepository
    {
        private DissimilisDbContext context;
        private BarRepository barRepository;

        public PartRepository(DissimilisDbContext context)
        {
            this.context = context;
            this.barRepository = new BarRepository(context);
        }

        /// <summary>
        /// Get part by id 
        /// </summary>
        /// <param name="partId"></param>
        /// <returns>PartDTO</returns>
        public async Task<PartDTO> GetPart(int partId)
        {
            Part ExistsPart = await this.context.Parts
                .Include(p => p.Instrument)
                .SingleOrDefaultAsync(p => p.Id == partId);

            PartDTO PartObject = null;

            if (ExistsPart != null)
            {
                PartObject = new PartDTO(ExistsPart);
                PartObject.Bars = await GetAllBarsForParts(ExistsPart.Id);
            }

            return PartObject;
        }


        /// <summary>
        /// Create a new Part to a Song
        /// </summary>
        /// <param name="NewPartObject"></param>
        /// <param name="userId"></param>
        /// <returns>SuperDTO</returns>
        public async Task<int> CreatePart(NewPartDTO NewPartObject, uint userId)
        {
            //Check if values are present in DTO, return 0 if one is missing
            if (!CheckProperties(NewPartObject)) return 0;

            var ExistsSong = await this.context.Songs
                .SingleOrDefaultAsync(s => s.Id == NewPartObject.SongId);
            if (!ValidateUser(userId, ExistsSong)) return 0;

            Part CheckPartNumber = await this.context.Parts
                .SingleOrDefaultAsync(b => b.PartNumber == NewPartObject.PartNumber
                                        && b.SongId == NewPartObject.SongId);
            if (CheckPartNumber != null)
            {
                UpdatePartNumbers(CheckPartNumber.PartNumber, CheckPartNumber.SongId, userId);
            }

            var ExistsInstrument = await CreateOrFindInstrument(NewPartObject.Title, userId);
            // This will trigger BadRequest from controller, Will remove when we have exception handler
            if (ExistsInstrument == null)
                return 0;

            int result = 0;

            if (ExistsSong != null)
            {
                var PartModelObject = new Part(ExistsSong.Id, ExistsInstrument.Id, NewPartObject.PartNumber);

                await this.context.Parts.AddAsync(PartModelObject);
                this.context.UserId = userId;
                await this.context.SaveChangesAsync();
                result = PartModelObject.Id;
            }
            return result;
        }

        /// <summary>
        /// Update the Part Numbers
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="songId"></param>
        /// <param name="userId"></param>
        private async void UpdatePartNumbers(int partNumber, int songId, uint userId)
        {
            Part[] AllParts = this.context.Parts.Where(b => b.SongId == songId)
                .OrderBy(x => x.PartNumber)
                .ToArray();

            for (int i = partNumber - 1; i < AllParts.Count(); i++)
            {
                AllParts[i].PartNumber += 1;
            }

            this.context.UserId = userId;
            await this.context.SaveChangesAsync();
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
                this.context.UserId = userId;
                await this.context.SaveChangesAsync();
            }
            return ExistsInstrument;
        }

        /// <summary>
        /// Get all associated bars to partId
        /// </summary>
        /// <param name="partId"></param>
        /// <returns>BarDTOArray</returns>
        private async Task<BarDTO[]> GetAllBarsForParts(int partId)
        {
            int[] AllBars = this.context.Bars
                .Where(x => x.PartId == partId)
                .OrderBy(x => x.BarNumber)
                .Select(x => x.Id)
                .ToArray();

            BarDTO[] AllBarsDTO = new BarDTO[AllBars.Count()];

            for (int i = 0; i < AllBars.Count(); i++)
            {
                AllBarsDTO[i] = await this.barRepository.GetBar(AllBars[i]);
            }

            return AllBarsDTO;
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

            var PartModelObject = await this.context.Parts
                .Include(p => p.Song)
                .Include(p => p.Instrument)
                .SingleOrDefaultAsync(s => s.Id == UpdatePartObject.Id);

            if (PartModelObject != null)
            {
                if (ValidateUser(userId, PartModelObject.Song))
                {
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

                    this.context.UserId = userId;
                    Updated = await this.context.TrySaveChangesAsync();
                }
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

            var PartModelObject = await this.context.Parts
                .Include(p => p.Song)
                .SingleOrDefaultAsync(p => p.Id == partId);

            if (PartModelObject != null)
                if (ValidateUser(userId, PartModelObject.Song))
                {
                    this.context.Parts.Remove(PartModelObject);
                    Deleted = await this.context.TrySaveChangesAsync();
                }
            
            return Deleted;
        }
    }
}
