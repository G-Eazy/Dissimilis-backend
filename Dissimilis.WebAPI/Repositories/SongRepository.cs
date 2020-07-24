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
using Dissimilis.WebAPI.Repositories.Validators;

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
        /// Get song by id
        /// </summary>
        /// <param name="songId"></param>
        /// <returns></returns>
        public async Task<SongDTO> GetSongById(int songId)
        {
            if (songId <= 0) return null;

            var SongModelObject = await this.context.Songs
                .Include(x => x.Arranger)
                .SingleOrDefaultAsync(s => s.Id == songId);
            if (SongModelObject is null) return null;

            SongDTO SongObject = new SongDTO(SongModelObject);
            SongObject.Voices = await GetAllPartsForSong(SongModelObject.Id);

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
            if (! IsValidDTO<NewSongDTO, NewSongDTOValidator>(NewSongObject)) return 0;

            var SongModelObject = new Song()
            {
                Title = NewSongObject.Title,
                ArrangerId = (int)userId,
                TimeSignature = NewSongObject.TimeSignature
            };

            await this.context.Songs.AddAsync(SongModelObject);
            this.context.UserId = userId;
            await this.context.SaveChangesAsync();

            return SongModelObject.Id;
        }

        /// <summary>
        /// Create a full song with all its parts
        /// </summary>
        /// <param name="songObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CreateFullSong(UpdateSongDTO songObject, uint userId)
        {
            //Get song and check if you are allowed to change it
            if (songObject is null)
                return 0;

            int NewSongId = await CreateSong(songObject, userId);

            //Create all the associated parts for this song
            bool PartsCreated = await this.partRepository.CreateAllParts(NewSongId, songObject.Voices, userId);

            //If all parts are created return songId, if there was an error, delete them all and return 0
            if (PartsCreated) return NewSongId;
            else
            {
                bool WasDeleted = await this.partRepository.DeleteParts(NewSongId, userId);
                return 0;
            }

        }

        /// <summary>
        /// Put/update a full song with all it's parts
        /// </summary>
        /// <param name="songObject"></param>
        /// <param name="userId"></param>
        /// <param name="songId"></param>
        /// <returns></returns>
        public async Task<int> UpdateSong(UpdateSongDTO songObject, uint userId, int songId)
        {
            //Get song and check if you are allowed to change it
            Song SongModel = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == songId);
            if (!ValidateUser(userId, SongModel)) return 0;

            //Update song, if it wasn't updated return 0
            bool WasUpdated = await UpdateSong(songObject, userId);
            if (!WasUpdated) return 0;
            
            //Delete all the parts under this song
            bool WasDeleted = await this.partRepository.DeleteParts(songId, userId);
            if (!WasDeleted) return 0;

            bool PartsCreated = await this.partRepository.CreateAllParts(songId, songObject.Voices, userId);
            
            //If all parts are created return songId, if there was an error, delete them all and return 0
            if (PartsCreated)
                return songId;
            else
            {
                await this.partRepository.DeleteParts(songId, userId);
                return 0;
            }
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

            if (! IsValidDTO<UpdateSongDTO, UpdateSongDTOValidator>(UpdateSongObject)) return Updated;
           
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == UpdateSongObject.Id);

            if (SongModelObject != null && ValidateUser(userId, SongModelObject))
            {
                if (UpdateSongObject.Title != SongModelObject.Title) SongModelObject.Title = UpdateSongObject.Title;
                if (UpdateSongObject.TimeSignature != SongModelObject.TimeSignature) SongModelObject.TimeSignature = UpdateSongObject.TimeSignature;

                this.context.UserId = userId;
                Updated = await this.context.TrySaveChangesAsync();
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
            if (songId <= 0) return Deleted;
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == songId);

            if (SongModelObject != null && ValidateUser(userId, SongModelObject))
            {
                this.context.Songs.Remove(SongModelObject);
                Deleted = await this.context.TrySaveChangesAsync();
            }

            return Deleted;
        }

        /// <summary>
        /// Get all the parts belonging to this song
        /// </summary>
        /// <param name="songId"></param>
        /// <returns></returns>
        public async Task<PartDTO[]> GetAllPartsForSong(int songId)
        {
            //get all parts belonging to this songid, decending by partnumber
            var AllParts = this.context.Parts
                .Where(x => x.SongId == songId)
                .OrderBy(x => x.PartNumber)
                .Include(p => p.Instrument)
                .Select(p => new PartDTO(p))
                .ToArray();

            var PartIds = AllParts.Select(x => x.Id);

            BarDTO[] AllBars = this.context.Bars
                .Where(b => PartIds.Contains(b.PartId))
                .OrderBy(b => b.BarNumber)
                .Select(b => new BarDTO(b))
                .ToArray();

            var BarIds = AllBars.Select(x => x.Id);

            var AllNotes = this.context.Notes
                .Where(n => BarIds.Contains(n.BarId))
                .OrderBy(n => n.NoteNumber)
                .Select(n => new NoteDTO(n))
                .ToArray();

            foreach (var bar in AllBars)
            {
                bar.Notes = AllNotes.Where(x => x.BarId == bar.Id).ToArray();
            }

            foreach (var part in AllParts)
            {
                part.Bars = AllBars.Where(x => x.PartId == part.Id).ToArray();
            }

            return AllParts;
        }
    }
}
