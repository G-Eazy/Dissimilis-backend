using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class SongRepository 
    {
        private DissimilisDbContext context;
        public SongRepository(DissimilisDbContext context)
        {
            this.context = context;
        }


        public async Task<SongDTO> GetSongByIdQuery(SuperDTO SuperObject)
        {
            var SongId = SuperObject.Id;
            var SongModelObject = await this.context.Songs
                .SingleOrDefaultAsync(s => s.Id == SongId);
            
            SongDTO SongObject = null;
            if (SongModelObject != null)
                SongObject = new SongDTO(SongModelObject);
            return SongObject;
        }

        public async Task<SongDTO[]> SearchQuery(SongSearchDTO SongSearchObject) {

            var Title = SongSearchObject.Title;
            var ArrangerId = SongSearchObject.ArrangerId;
            var Num = SongSearchObject.Num;
            bool OrderByDateTime = SongSearchObject.OrderByDateTime;
            var SongQuery = this.context.Songs.AsQueryable();

            if (! String.IsNullOrEmpty(Title))
                SongQuery = SongQuery
                    .Where(s => s.Title.Contains(Title))
                    .AsQueryable();
            if (ArrangerId != 0) 
                SongQuery = SongQuery
                    .Where(s => s.ArrangerId == ArrangerId)
                    .AsQueryable();
            if (Num != 0)
                SongQuery = SongQuery
                    .Take((int) Num)
                    .AsQueryable();
            if (OrderByDateTime)
                SongQuery = SongQuery
                    .OrderByDescending(s => s.UpdatedOn);

            var SongModelArray = await SongQuery
                .ToArrayAsync();

            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();
            return SongDTOArray;
        
        }

        public async Task<SongDTO> CreateSongCommand(NewSongDTO NewSongObject)
        {
            var ArrangerId = NewSongObject.ArrangerId;
            var ExistsArranger = await this.context.Users.SingleOrDefaultAsync(u => u.Id == ArrangerId);
            SongDTO SongObject = null;
            if (ExistsArranger != null)
            { 
                var SongModelObject = new Song()
                {
                    Title = NewSongObject.Title,
                    ArrangerId = NewSongObject.ArrangerId
                };
                await this.context.Songs.AddAsync(SongModelObject);
                await this.context.SaveChangesAsync();
                SongObject = new SongDTO(SongModelObject);
            }
            return SongObject;
        }
        public async Task<bool> UpdateSongCommand(UpdateSongDTO UpdateSongObject)
        {
            var UpdateSongObjectId = UpdateSongObject.Id;
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == UpdateSongObjectId);
            bool Updated =  false;
            if (SongModelObject != null) 
            {
                SongModelObject.UpdatedOn = DateTime.UtcNow;
                await this.context.SaveChangesAsync();
                Updated = true;
            }
            return Updated;
        }

        public async Task<bool> DeleteSongCommand(SuperDTO DeleteSongObject)
        {
            var DeleteSongObjectId = DeleteSongObject.Id;
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == DeleteSongObjectId);
            bool Deleted = false;
            if (SongModelObject != null) 
            { 
                this.context.Songs.Remove(SongModelObject);
                await this.context.SaveChangesAsync();
                Deleted = true;
            }
            return Deleted;
        }
    }
}
