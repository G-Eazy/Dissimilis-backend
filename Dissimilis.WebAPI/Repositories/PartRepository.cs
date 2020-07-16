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
        public PartRepository(DissimilisDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Create a new Part to a Song
        /// </summary>
        /// <param name="NewPartObject"></param>
        /// <returns>SuperDTO</returns>

        public async Task<SuperDTO> CreatePartCommand(NewPartDTO NewPartObject)
        {
            var SongId = NewPartObject.SongId;
            var ExistsSong = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == SongId);
            var ExistsInstrument = await CreateOrFindInstrument(NewPartObject.Title);
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
                await this.context.SaveChangesAsync();
                PartObject = new SuperDTO(SongId);
                // This will be changed to a better solution, but i need to return both SongId and PartId
                PartObject.Id2 = NewPartObject.Priority;
            }
            return PartObject;
        }
        /// <summary>
        /// Looks for an instrument with title InstrumentName, and creates if non-existant
        /// </summary>
        /// <param name="InstrumentName"></param>
        /// <returns>(Model) Instrument</returns>
        public async Task<Instrument> CreateOrFindInstrument(string InstrumentName)
        {
            if (String.IsNullOrWhiteSpace(InstrumentName))
                //throw new ArgumentNullException(nameof(InstrumentName)); // Commenting out until we have exception handler
                return null;

            var ExistsInstrument = await this.context.Instruments.SingleOrDefaultAsync(i => i.Name == InstrumentName);
            if (ExistsInstrument is null) {
                ExistsInstrument = new Instrument(InstrumentName);
                await this.context.Instruments
                    .AddAsync(ExistsInstrument);
                await this.context.SaveChangesAsync();
            }
                return ExistsInstrument;
        }

    }
}
