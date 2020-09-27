using Dissimilis.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;


namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class Repository
    {
        private readonly DissimilisDbContext context;

        public Repository(DissimilisDbContext context)
        {
            this.context = context;
        }


        public async Task<SongBar> GetBarById(int songId, int partId, int barId, CancellationToken cancellationToken)
        {
            var bar = await context.SongBars
                .Include(b => b.Notes)
                .Where(b => b.SongVoice.SongId == songId && b.PartId == partId)
                .FirstOrDefaultAsync(x => x.Id == barId, cancellationToken);

            if (bar == null)
            {
                throw new NotFoundException($"Bar with ID {barId} not found.");
            }

            return bar;
        }


        /// <summary>
        /// Update Bar numbers
        /// </summary>
        /// <param name="barNumber"></param>
        /// <param name="partId"></param>
        /// <param name="userId"></param>
        private async Task<bool> UpdateBarNumbers(int barNumber, int partId, uint userId)
        {
            SongBar[] allbars = this.context.SongBars.Where(b => b.PartId == partId)
                .OrderBy(x => x.BarNumber)
                .ToArray();

            for (int i = barNumber - 1; i < allbars.Count(); i++)
            {
                allbars[i].BarNumber++;
            }


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

            SongBar songBarModel = await this.context.SongBars
                .Include(b => b.SongVoice)
                .ThenInclude(b => b.Song)
                .SingleOrDefaultAsync(x => x.Id == barId);
            if (songBarModel != null && ValidateUser(userId, songBarModel.SongVoice.Song))
            {
                this.context.Remove(songBarModel);
                await context.SaveChangesAsync();
            }

            return Deleted;
        }

        /// <summary>
        /// Get all the notes associated with this bar
        /// </summary>
        /// <param name="barId"></param>
        /// <returns></returns>
        private async Task<NoteDto[]> FindAllNotesForBar(int barId)
        {
            var AllNotes = this.context.SongNotes
                           .Where(n => n.BarId == barId)
                           .OrderBy(n => n.NoteNumber)
                           .Select(n => new NoteDto(n))
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

            if (!IsValidDTO<UpdateBarDTO, UpdateBarDTOValidator>(UpdateBarObject)) return Updated;

            SongBar songBarModel = await this.context.SongBars
                .Include(x => x.SongVoice)
                .ThenInclude(x => x.Song)
                .SingleOrDefaultAsync(b => b.Id == UpdateBarObject.Id);

            if (songBarModel != null && ValidateUser(userId, songBarModel.SongVoice.Song))
            {
                SongBar checkSongBarNumber = await this.context.SongBars.SingleOrDefaultAsync(b => b.BarNumber == UpdateBarObject.BarNumber && b.PartId == UpdateBarObject.PartId);
                if (checkSongBarNumber != null)
                {
                    await UpdateBarNumbers(UpdateBarObject.BarNumber, UpdateBarObject.PartId, userId);
                }

                if (UpdateBarObject.BarNumber != songBarModel.BarNumber) songBarModel.BarNumber = UpdateBarObject.BarNumber;
                if (UpdateBarObject.RepAfter != songBarModel.RepAfter) songBarModel.RepAfter = UpdateBarObject.RepAfter;
                if (UpdateBarObject.RepBefore != songBarModel.RepBefore) songBarModel.RepBefore = UpdateBarObject.RepBefore;
                if (UpdateBarObject.House != songBarModel.House) songBarModel.House = UpdateBarObject.House;


                await this.context.SaveChangesAsync();
            }

            return Updated;
        }

        public async Task SaveAsync(SongBar bar, CancellationToken cancellationToken)
        {
            await context.SongBars.AddAsync(bar, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<SongVoice> GetPartById(int songId, int partId, CancellationToken cancellationToken)
        {
            var part = await context.SongParts
                .Include(p => p.Bars)
                .Where(p => p.SongId == songId)
                .FirstOrDefaultAsync(p => p.Id == partId, cancellationToken);

            if (part == null)
            {
                throw new NotFoundException($"Part with SongId {songId} and PartId {partId} not found.");
            }

            return part;
        }
    }
}
