using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories
{
    public class PartRepository 
    {
        private DissimilisDbContext context;
        private BarRepository barRepository;

        public PartRepository(DissimilisDbContext context)
        {
            this.barRepository = new BarRepository(context);
            this.context = context;
        }

        /// <summary>
        /// Get part by id provided in DTO
        /// </summary>
        /// <param name="partId"></param>
        /// <returns>PartDTO</returns>
        public async Task<PartDTO> GetPartById(int partId)
        {
            Part ExistsPart = await this.context.Parts
                .Include(p => p.Instrument)
                .SingleOrDefaultAsync(p => p.Id == partId);

            PartDTO PartObject = null;

            if (ExistsPart != null)
            {
                PartObject = new PartDTO(ExistsPart);
                PartObject.Bars = await GetAllBars(ExistsPart.Id);
            }

            return PartObject;
        }

            /// <summary>
            /// Create a new Part to a Song
            /// </summary>
            /// <param name="NewPartObject"></param>
            /// <param name="userId"></param>
            /// <returns>SuperDTO</returns>

        public async Task<SuperDTO> CreatePartCommand(NewPartDTO NewPartObject, uint userId)
        {
            //if dto is empty, return null
            if (NewPartObject is null) return null;
            var SongId = NewPartObject.SongId;
            
            var ExistsSong = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == SongId);
            if (!ValidateUser(userId, ExistsSong)) return null;

            Part CheckPartNumber = await this.context.Parts.SingleOrDefaultAsync(b => b.PartNumber == NewPartObject.Priority && b.SongId == NewPartObject.SongId);
            if (CheckPartNumber != null)
            {
                UpdatePartNumbers(CheckPartNumber.PartNumber, CheckPartNumber.SongId, userId);
            }

            var ExistsInstrument = await CreateOrFindInstrument(NewPartObject.Title, userId);
            // This will trigger BadRequest from controller, Will remove when we have exception handler
            if (ExistsInstrument == null)
                return null;

            SuperDTO PartObject = null;

            if (ExistsSong != null)
            {
                var PartModelObject = new Part()
                {
                    SongId = ExistsSong.Id,
                    InstrumentId = ExistsInstrument.Id,
                    PartNumber = NewPartObject.Priority
                };
                await this.context.Parts.AddAsync(PartModelObject);
                this.context.UserId = userId;
                await this.context.SaveChangesAsync();
                PartObject = new SuperDTO(NewPartObject.Priority);
            }
            return PartObject;
        }

        public async void UpdatePartNumbers(int partNumber, int songId, uint userId)
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

            var ExistsInstrument = await this.context.Instruments.SingleOrDefaultAsync(i => i.Name == InstrumentName);
            if (ExistsInstrument is null) {
                ExistsInstrument = new Instrument(InstrumentName);
                await this.context.Instruments
                    .AddAsync(ExistsInstrument);
                this.context.UserId = userId;
                await this.context.SaveChangesAsync();
            }
                return ExistsInstrument;
        }

        public async Task<BarDTO[]> GetAllBars(int partId)
        {
            Bar[] AllBars = this.context.Bars.Where(x => x.PartId == partId)
                .OrderBy(x => x.BarNumber).ToArray();

            BarDTO[] AllBarsDTO = new BarDTO[AllBars.Count()];

            for(int i = 0; i < AllBars.Count(); i++)
            {
                AllBarsDTO[i] = await this.barRepository.GetBar(AllBars[i].Id);
            }

            return AllBarsDTO;
        }

        /// <summary>
        /// Check if the user belongs to the bar it is trying to access/edit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        public bool ValidateUser(uint userId, Song song)
        {
            try
            {
                if (userId == song.CreatedById)
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
