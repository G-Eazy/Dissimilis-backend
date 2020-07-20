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
            SongObject.Parts = GetAllPartsForSong(SongModelObject.Id);

            return SongObject;
        }

        /// <summary>
        /// Search songs with parameters in SongSearchDTO
        /// </summary>
        /// <param name="SongQueryObject"></param>
        /// <returns></returns>
        public async Task<SongDTO[]> SearchSongs(SongQueryDTO SongQueryObject) {

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
            //TODO CHECK IF DTO IS EMPTY!
            var ArrangerId = NewSongObject.ArrangerId;
            var ExistsArranger = await this.context.Users.SingleOrDefaultAsync(u => u.Id == ArrangerId);
            SongDTO SongObject = null;
            if (ExistsArranger != null)
            {
                var SongModelObject = new Song()
                {
                    Title = NewSongObject.Title,
                    ArrangerId = NewSongObject.ArrangerId,
                    TimeSignature = NewSongObject.TimeSignature
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
            bool Updated = false;
            //TODO VALIDATE USER; CHECK IF DTO IS EMPTY!
            var UpdateSongObjectId = UpdateSongObject.Id;
            var NewTitle = UpdateSongObject.Title;
            var NewTimeSignature = UpdateSongObject.TimeSignature;

            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == UpdateSongObjectId);
            
            if (SongModelObject != null) 
            {
                SongModelObject.Title = NewTitle;
                SongModelObject.TimeSignature = NewTimeSignature;
                this.context.UserId = userId;
                Updated = await this.context.TrySaveChangesAsync();
            }
            return Updated;
        }

        /// <summary>
        /// Delete song using deletesong DTO
        /// </summary>
        /// <param name="DeleteSongObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSong(SuperDTO DeleteSongObject, int userId)
        {
            bool Deleted = false;
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == DeleteSongObject.Id);
            if (ValidateUser(userId, SongModelObject))
                if (SongModelObject != null) 
            { 
                this.context.Songs.Remove(SongModelObject);
                Deleted = await this.context.TrySaveChangesAsync();
            }

            return Deleted;
        }

        public bool ValidateUser(int userId, Song song)
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

        public PartDTO[] GetAllPartsForSong(int songId)
        {
            //get all parts belonging to this songid, decending by partnumber
            Part[] AllParts = this.context.Parts.Where(x => x.SongId == songId)
                .OrderBy(x => x.PartNumber).ToArray();

            PartDTO[] AllPartsDTO = new PartDTO[AllParts.Length];

            for(int i = 0; i < AllParts.Length; i++)
            {
                AllPartsDTO[i] = new PartDTO(AllParts[i].Id, AllParts[i].PartNumber, AllParts[i].SongId, AllParts[i].InstrumentId);
            }
            
            return AllPartsDTO;
                
        }
    }
}
