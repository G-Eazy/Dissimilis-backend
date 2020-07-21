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
    public class SongRepository : BaseRepository, ISongRepository
    {
        private DissimilisDbContext context;
        private PartRepository partRepository;

        public SongRepository(DissimilisDbContext context)
        {
            this.context = context;
            this.partRepository = new PartRepository(context);
        }

        /// <summary>
        /// Get song by id provided in DTO
        /// </summary>
        /// <param name="songId"></param>
        /// <returns></returns>
        public async Task<SongDTO> GetSongById(int songId)
        {
            var SongModelObject = await this.context.Songs
                .Include(x => x.Arranger)
                .SingleOrDefaultAsync(s => s.Id == songId);
            if (SongModelObject is null) return null;

            SongDTO SongObject = new SongDTO(SongModelObject);
            SongObject.Parts = await GetAllPartsForSong(SongModelObject.Id);

            return SongObject;
        }

        /// <summary>
        /// Search songs with parameters in SongSearchDTO
        /// </summary>
        /// <param name="SongQueryObject"></param>
        /// <returns></returns>
        public async Task<UpdateSongDTO[]> SearchSongs(SongQueryDTO SongQueryObject) {

            var Title = SongQueryObject.Title;
            var ArrangerId = SongQueryObject.ArrangerId;
            var Num = SongQueryObject.Num;
            bool OrderByDateTime = SongQueryObject.OrderByDateTime;
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

            var SongDTOArray = SongModelArray.Select(u => new UpdateSongDTO(u)).ToArray();
            return SongDTOArray;
        
        }

        /// <summary>
        /// Create song using NewSongDTO
        /// </summary>
        /// <param name="NewSongObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CreateSong(NewSongDTO NewSongObject, uint userId)
        {
            Console.WriteLine(NewSongObject.Title);
            var SongModelObject = new Song()
            {
                Title = NewSongObject.Title,
                ArrangerId = (int)userId,
                TimeSignature = NewSongObject.TimeSignature
            };
            await this.context.Songs.AddAsync(SongModelObject);
            this.context.UserId = userId;
            await this.context.SaveChangesAsync();
            var result = SongModelObject.Id;

            return result;
        }

        /// <summary>
        /// UpdateSong using UpdateSongDTO
        /// </summary>
        /// <param name="UpdateSongObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSong(UpdateSongDTO UpdateSongObject, uint userId)
        {
            bool Updated = false;
            var UpdateSongObjectId = UpdateSongObject.Id;
            var NewTitle = UpdateSongObject.Title;
            var NewTimeSignature = UpdateSongObject.TimeSignature;

            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == UpdateSongObjectId);

            if (SongModelObject != null) 
            {
                if (ValidateUser(userId, SongModelObject))
                {
                    SongModelObject.Title = NewTitle;
                    SongModelObject.TimeSignature = NewTimeSignature;
                    this.context.UserId = userId;
                    Updated = await this.context.TrySaveChangesAsync();
                }
            }
            return Updated;
        }

        /// <summary>
        /// Delete song by songId
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="userId"></param>
        /// <returns>boolean</returns>
        public async Task<bool> DeleteSong(int songId, uint userId)
        {
            bool Deleted = false;
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == songId);

            if (SongModelObject != null)
                if (ValidateUser(userId, SongModelObject))
                {
                    this.context.Songs.Remove(SongModelObject);
                    Deleted = await this.context.TrySaveChangesAsync();
                }

            return Deleted;
        }

        public async Task<PartDTO[]> GetAllPartsForSong(int songId)
        {
            //get all parts belonging to this songid, decending by partnumber
            Part[] AllParts = this.context.Parts.Where(x => x.SongId == songId)
                .OrderBy(x => x.PartNumber).ToArray();

            PartDTO[] AllPartsDTO = new PartDTO[AllParts.Length];

            for(int i = 0; i < AllParts.Length; i++)
            {
                AllPartsDTO[i] = await this.partRepository.GetPartById(AllParts[i].Id);
            }
            
            return AllPartsDTO;
                
        }
    }
}
