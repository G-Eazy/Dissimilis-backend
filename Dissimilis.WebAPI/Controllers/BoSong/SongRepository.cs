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

        public async Task<SongDTO[]> AllSongsQuery()
        {
            var SongModelArray = await this.context.Songs.ToArrayAsync();
            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();
            return SongDTOArray;
        }
        
        public async Task<SongDTO[]> FilteredSongsQuery(string query)
        {
            string Query = query;
            var SongModelArray = await this.context.Songs
                .Where(s => s.Title.Contains(Query))
                .ToArrayAsync(); ;

            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();
            return SongDTOArray;
        }
        public async Task<SongDTO[]> SongsByArrangerQuery(SongsByArrangerDTO SongsByArrangerObject)
        {
            var Num = SongsByArrangerObject.Num;
            var ArrangerId = SongsByArrangerObject.ArrangerId;
            bool OrderByDateTime = SongsByArrangerObject.OrderByDateTime;

            var SongQuery = this.context.Songs
                .Where(s => s.ArrangerId == ArrangerId)
                .AsQueryable();

            if (OrderByDateTime)
                SongQuery = SongQuery
                    .OrderByDescending(s => s.UpdatedOn);

            var SongModelArray = await SongQuery
                .Take(Num)
                .ToArrayAsync();

            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();
            return SongDTOArray;
        }

        public async Task<SongDTO> CreateSongCommand(NewSongDTO NewSongObject)
        {
            var ArrangerId = NewSongObject.ArrangerId;
            var ExistsArranger = await this.context.Users.FirstOrDefaultAsync(u => u.Id == ArrangerId);
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
        public async Task<SuperDTO> UpdateSongCommand(UpdateSongDTO UpdateSongObject)
        {
            var UpdateSongObjectId = UpdateSongObject.Id;
            var SongModelObject = await this.context.Songs.FirstOrDefaultAsync(s => s.Id == UpdateSongObjectId);
            SuperDTO SongObject = null;
            if (SongModelObject != null) 
            {
                SongModelObject.UpdatedOn = DateTime.UtcNow;
                await this.context.SaveChangesAsync();
                SongObject = new SuperDTO(SongModelObject.Id);
            }
            return SongObject;
        }

        public async Task<SuperDTO> DeleteSongCommand(SuperDTO DeleteSongObject)
        {
            var DeleteSongObjectId = DeleteSongObject.Id;
            var SongModelObject = await this.context.Songs.FirstOrDefaultAsync(s => s.Id == DeleteSongObjectId);
            SuperDTO SongObject = null;
            if (SongModelObject != null) 
            { 
                this.context.Songs.Remove(SongModelObject);
                await this.context.SaveChangesAsync();
                SongObject = new SuperDTO(SongModelObject.Id);
            }
            return SongObject;
        }
    }
}
