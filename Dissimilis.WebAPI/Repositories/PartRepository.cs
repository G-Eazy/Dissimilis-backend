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
        /// Get part by id provided in DTO
        /// </summary>
        /// <param name="SuperObject"></param>
        /// <returns>PartDTO</returns>
        public async Task<PartDTO> GetPartById(SuperDTO SuperObject)
        {
            var PartId = SuperObject.Id;
            Part ExistsPart = await this.context.Parts
                .Include(p => p.Instrument)
                .SingleOrDefaultAsync(p => p.Id == PartId);
            
            PartDTO PartObject = null;

            if (ExistsPart != null)
                PartObject = new PartDTO(ExistsPart);

            return PartObject;
        }

        /// <summary>
        /// Create a new Part to a Song
        /// </summary>
        /// <param name="NewPartObject"></param>
        /// <returns>SuperDTO</returns>

        public async Task<SuperDTO> CreatePartCommand(NewPartDTO NewPartObject, uint userId)
        {
            //if dto is empty, return null
            if (NewPartObject is null) return null;
            var SongId = NewPartObject.SongId;
            
            var ExistsSong = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == SongId);
            if (!ValidateUser(userId, ExistsSong)) return null;

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
                PartObject = new SuperDTO(PartModelObject.Id);
            }
            return PartObject;
        }
        /// <summary>
        /// Looks for an instrument with title InstrumentName, and creates if non-existant
        /// </summary>
        /// <param name="InstrumentName"></param>
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

        /// <summary>
        /// UpdatePart using UpdatePartDTO
        /// </summary>
        /// <param name="UpdatePartObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePartCommand(UpdatePartDTO UpdatePartObject, uint userId)
        {
            bool Updated = false;

            var PartModelObject = await this.context.Parts
                .Include(p => p.Song)
                .Include(p => p.Instrument)
                .SingleOrDefaultAsync(s => s.Id == UpdatePartObject.Id);

            if (ValidateUser(userId, PartModelObject.Song)) 
            { 
                if (PartModelObject != null) 
                {
                    // Checking for differences between Model and DTO 
                    if (PartModelObject.Instrument.Name != UpdatePartObject.Title)
                    {
                        var NewInstrument = await CreateOrFindInstrument(UpdatePartObject.Title, userId);
                        PartModelObject.InstrumentId = NewInstrument.Id;
                    }
                    if (PartModelObject.PartNumber != UpdatePartObject.Priority)
                    {
                        PartModelObject.PartNumber = UpdatePartObject.Priority;
                    }

                    this.context.UserId = userId;
                    Updated = await this.context.TrySaveChangesAsync();
                }
            }
            return Updated;
        }


        
        /// <summary>
        /// Delete Part by Id
        /// </summary>
        /// <param name="DeletePartObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePart(SuperDTO DeletePartObject, uint userId)
        {
            bool Deleted = false;
            var PartModelObject = await this.context.Parts
                .Include(p => p.Song)
                .SingleOrDefaultAsync(p => p.Id == DeletePartObject.Id);
            if (PartModelObject is null)
                return Deleted;

            if (ValidateUser(userId, PartModelObject.Song))
                if (PartModelObject != null) 
                { 
                    this.context.Parts.Remove(PartModelObject);
                    this.context.UserId = userId;                        
                    Deleted = await this.context.TrySaveChangesAsync();
                }

            return Deleted;
        }

        /// <summary>
        /// Check if the user belongs to the entity it is trying to access/edit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        public bool ValidateUser(uint userId, BaseEntity entity)
        {
            try
            {
                if (userId == entity.CreatedById)
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
