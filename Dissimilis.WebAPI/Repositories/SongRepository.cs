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
    public class SongRepository : ISongRepository
    {
        private DissimilisDbContext context;
        public SongRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get song by id provided in DTO
        /// </summary>
        /// <param name="SuperObject"></param>
        /// <returns></returns>
        public async Task<SongDTO> GetSongById(SuperDTO SuperObject)
        {
            var SongId = SuperObject.Id;
            var SongModelObject = await this.context.Songs
                .SingleOrDefaultAsync(s => s.Id == SongId);
            
            SongDTO SongObject = new SongDTO(SongModelObject);

            return SongObject;
        }

        /// <summary>
        /// Search songs with parameters in SongSearchDTO
        /// </summary>
        /// <param name="SongSearchObject"></param>
        /// <returns></returns>
        public async Task<SongDTO[]> SearchSongs(SongSearchDTO SongSearchObject) {

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

        /// <summary>
        /// Create song using NewSongDTO
        /// </summary>
        /// <param name="NewSongObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SongDTO> CreateSong(NewSongDTO NewSongObject, uint userId)
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
                this.context.UserId = userId;
                await this.context.SaveChangesAsync();
                SongObject = new SongDTO(SongModelObject);
            }
            return SongObject;
        }

        /// <summary>
        /// UpdateSong using UpdateSongDTO
        /// </summary>
        /// <param name="UpdateSongObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSong(UpdateSongDTO UpdateSongObject, uint userId)
        {
            var UpdateSongObjectId = UpdateSongObject.Id;
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == UpdateSongObjectId);
            bool Updated =  false;
            if (SongModelObject != null) 
            {
                this.context.UserId = userId;
                await this.context.SaveChangesAsync();
                Updated = true;
            }
            return Updated;
        }

        /// <summary>
        /// Delete song using deletesong DTO
        /// </summary>
        /// <param name="DeleteSongObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSong(SuperDTO DeleteSongObject, uint userId)
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
