﻿using Dissimilis.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using BarDto = Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut.BarDto;

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
        public async Task<SongByIdDto> GetSongById(int songId)
        {
            if (songId <= 0) return null;

            var SongModelObject = await this.context.Songs
                .Include(x => x.Arranger)
                .SingleOrDefaultAsync(s => s.Id == songId);
            if (SongModelObject is null) return null;

            SongByIdDto Object = new SongByIdDto(SongModelObject);
            Object.Voices = await GetAllPartsForSong(SongModelObject.Id);

            return Object;
        }

        /// <summary>
        /// Search songs with parameters in SongSearchDTO
        /// </summary>
        /// <param name="SongQueryObject"></param>
        /// <returns></returns>
        public async Task<UpdateSongDto[]> SearchSongs(SearchQueryDto SongQueryObject) {

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

            var SongDTOArray = SongModelArray.Select(u => new UpdateSongDto(u)).ToArray();
            return SongDTOArray;
        
        }
        
        /// <summary>
        /// Create song using NewSongDTO
        /// </summary>
        /// <param name="createSongObject"></param>
        /// <param name="userId"></param>
        /// <returns>SongDTO</returns>
        public async Task<SongByIdDto> CreateSong(CreateSongDto createSongObject, uint userId)
        {
            if (! IsValidDTO<CreateSongDto, NewSongDTOValidator>(createSongObject)) return null;

            var SongModelObject = new Song()
            {
                Title = createSongObject.Title,
                ArrangerId = (int)userId,
                TimeSignature = createSongObject.TimeSignature
            };

            await this.context.Songs.AddAsync(SongModelObject);
            await this.context.SaveChangesAsync();

            return new SongByIdDto(SongModelObject);
        }

        /// <summary>
        /// Create song using NewSongDTO with an empty first part
        /// </summary>
        /// <param name="createSongObject"></param>
        /// <param name="userId"></param>
        /// <returns>SongDTO</returns>
        public async Task<SongByIdDto> CreateSongWithPart(CreateSongDto createSongObject, uint userId)
        {
            if (! IsValidDTO<CreateSongDto, NewSongDTOValidator>(createSongObject)) return null;

            var SongModelObject = new Song()
            {
                Title = createSongObject.Title,
                ArrangerId = (int)userId,
                TimeSignature = createSongObject.TimeSignature
            };
            await this.context.Songs.AddAsync(SongModelObject);
            
            await this.context.SaveChangesAsync();

            // Creating partiture and linking to Song
            CreatePartDto createPartObject = new CreatePartDto()
            {
                SongId = SongModelObject.Id,
                Title = "Partitur", // Hardcoded for now, will be in NewSongDTO when frontend is ready
                PartNumber = 1
            };
            await this.partRepository.CreatePart(createPartObject, userId);
            await this.context.SaveChangesAsync();

            // Sending Song with first Part back as request body
            return await GetSongById(SongModelObject.Id);
        }


        /// <summary>
        /// Put/update a full song with all it's parts
        /// </summary>
        /// <param name="songObject"></param>
        /// <param name="userId"></param>
        /// <param name="songId"></param>
        /// <returns></returns>
        public async Task<SongByIdDto> UpdateSong(UpdateSongDto songObject, uint userId, int songId)
        {
            //Get song and check if you are allowed to change it
            Song SongModel = await this.context.Songs.Include(x => x.Arranger).SingleOrDefaultAsync(s => s.Id == songId);
            if (!ValidateUser(userId, SongModel)) return null;

            //Update song, if it wasn't updated return 0
            bool WasUpdated = await UpdateSong(songObject, userId);
            if (!WasUpdated) return null;

            //Delete all the parts under this song
            await this.partRepository.DeleteParts(songId, userId);
           // if (!WasDeleted) return null;

            bool PartsCreated = await this.partRepository.CreateAllParts(songId, songObject.Voices, userId);

            //If all parts are created return songId, if there was an error, delete them all and return 0
            if (PartsCreated)
            {
                await this.context.SaveChangesAsync();
                return new SongByIdDto(SongModel);
            }
            else
            {
                await this.partRepository.DeleteParts(songId, userId);
                return null;
            }
        }

        /// <summary>
        /// UpdateSong using UpdateSongDTO
        /// </summary>
        /// <param name="UpdateSongObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSong(UpdateSongDto UpdateSongObject, uint userId)
        {
            bool Updated = false;

            if (! IsValidDTO<UpdateSongDto, UpdateSongDTOValidator>(UpdateSongObject)) return Updated;
           
            var SongModelObject = await this.context.Songs.SingleOrDefaultAsync(s => s.Id == UpdateSongObject.Id);

            if (SongModelObject != null && ValidateUser(userId, SongModelObject))
            {
                if (UpdateSongObject.Title != SongModelObject.Title) SongModelObject.Title = UpdateSongObject.Title;
                if (UpdateSongObject.TimeSignature != SongModelObject.TimeSignature) SongModelObject.TimeSignature = UpdateSongObject.TimeSignature;

                await this.context.SaveChangesAsync();
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
                await this.context.SaveChangesAsync();
            }

            return Deleted;
        }

        /// <summary>
        /// Get all the parts belonging to this song
        /// </summary>
        /// <param name="songId"></param>
        /// <returns></returns>
        public async Task<PartDto[]> GetAllPartsForSong(int songId)
        {
            var AllParts = await this.context.SongParts
                .Where(x => x.SongId == songId)
                .OrderBy(x => x.PartNumber)
                .Include(p => p.Instrument)
                .Select(p => new PartDto(p))
                .ToArrayAsync();

            var PartIds = AllParts.Select(x => x.PartId);

            var AllBars = await this.context.SongBars
                .Where(b => PartIds.Contains(b.PartId))
                .OrderBy(b => b.BarNumber)
                .Select(b => new BarDto(b))
                .ToArrayAsync();

            var BarIds = AllBars.Select(x => x.BarId);

            var AllNotes = await this.context.SongNotes
                .Where(n => BarIds.Contains(n.BarId))
                .OrderBy(n => n.NoteNumber)
                .Select(n => new NoteDTO(n))
                .ToArrayAsync();


            foreach (var bar in AllBars)
            {
           
                bar.ChordsAndNotes = AllNotes.Where(x => x.BarId == bar.BarId).ToArray();
            }

            foreach (var part in AllParts)
            {
                part.Bars = AllBars.Where(x => x.PartId == part.PartId).ToArray();
            }

            return AllParts;
        }
    }
}
