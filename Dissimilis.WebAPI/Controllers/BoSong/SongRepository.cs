using Dissimilis.WebAPI.Controllers.BoSong.DTOs;
using Dissimilis.WebAPI.Controllers.SuperDTOs;
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

        private DissimilisDbContext _context;
        public SongRepository(DissimilisDbContext _context)
        {
            this._context = _context;
        }

        public async Task<SongDTO[]> AllSongsQuery()
        {
            var SongModelArray = await this._context.Songs.ToArrayAsync();
            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();
            return SongDTOArray;
        }
        
        public async Task<SongDTO[]> FilteredSongsQuery(string query)
        {
            string Query = query;

            var SongModelArray = await this._context.Songs
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

            var SongQuery = this._context.Songs
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

        public async Task<Song> CreateSongCommand(NewSongDTO NewSongObject)
        {
            var song= new Song()
            {
                Title = NewSongObject.Title,
                ArrangerId = NewSongObject.ArrangerId
            };
            await this._context.Songs.AddAsync(song);
            await this._context.SaveChangesAsync();
            return song;
        }
        public async Task<SuperDTO> UpdateSongCommand(UpdateSongDTO UpdateSongObject)
        {
            var UpdateSongObjectId = UpdateSongObject.Id;
            var SongObject = await this._context.Songs.FirstOrDefaultAsync(s => s.Id == UpdateSongObjectId);
            if (SongObject != null) 
            {
                SongObject.UpdatedOn = DateTime.UtcNow;
                await this._context.SaveChangesAsync();
                return new SuperDTO(SongObject.Id);
            }
            return null;
        }

        public async Task<SuperDTO> DeleteSongCommand(SuperDTO DeleteSongObject)
        {
            var DeleteSongObjectId = DeleteSongObject.Id;
            var SongModelObject = await this._context.Songs.FirstOrDefaultAsync(s => s.Id == DeleteSongObjectId);
            SuperDTO SongObject = null;
            if (SongModelObject != null) 
            { 
                this._context.Songs.Remove(SongModelObject);
                await this._context.SaveChangesAsync();
                SongObject = new SuperDTO(SongModelObject.Id);
            }
            return SongObject;
        }
    }
}
