﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.DbContext.Models.Enums;
using System;
using static Dissimilis.WebAPI.Extensions.Models.SongNoteExtension;
using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class SongRepository
    {

        internal DissimilisDbContext Context;
        public SongRepository(DissimilisDbContext context)
        {
            Context = context;
        }

        public async Task<Song> GetSongById(int songId, CancellationToken cancellationToken)
        {
            var song = await Context.Songs
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            await Context.SongVoices
                .Include(sv => sv.Instrument)
                .Where(sv => sv.SongId == songId)
                .LoadAsync(cancellationToken);

            await Context.SongBars
                .Include(sb => sb.Notes)
                .Where(sb => sb.SongVoice.SongId == songId)
                .LoadAsync(cancellationToken);

            return song;
        }

        public async Task<Song> GetFullSongById(int songId, CancellationToken cancellationToken)
        {
            var song = await Context.Songs
                .Include(s => s.Arranger)
                .Include(s => s.CreatedBy)
                .Include(s => s.UpdatedBy)
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            await Context.SongVoices
                .Include(p => p.Instrument)
                .Where(p => p.SongId == songId)
                .LoadAsync(cancellationToken);

            await Context.SongBars
                .Include(b => b.Notes)
                .Where(b => b.SongVoice.SongId == songId)
                .LoadAsync(cancellationToken);

            return song;
        }

        public async Task<Song[]> GetAllSongsInMyLibrary(int userId, CancellationToken cancellationToken)
        {
            var songs = await Context.Songs
                .Where(s => s.CreatedById == userId || s.ArrangerId == userId)
                .ToArrayAsync(cancellationToken);
            
            return songs;
        }

        public async Task SaveAsync(Song song, CancellationToken cancellationToken)
        {
            await Context.Songs.AddAsync(song, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Song> GetSongByIdForUpdate(int songId, CancellationToken cancellationToken)
        {
            var song = await Context.Songs
                .Include(s => s.Arranger)
                .Include(s => s.CreatedBy)
                .Include(s => s.UpdatedBy)
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            return song;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSong(Song song, CancellationToken cancellationToken)
        {
            Context.Songs.Remove(song);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Song[]> GetSongSearchList(User user, SearchQueryDto searchCommand, CancellationToken cancellationToken)
        {
            var query = Context.Songs
                .Include(s => s.Arranger)
                .Include(s => s.SharedGroups)
                .ThenInclude(sg => sg.Group)
                .AsQueryable()
                .Where(
                SongExtension.ReadAccessToSong(user)
                );


            if (!string.IsNullOrEmpty(searchCommand.Title))
            {
                var textSearch = $"%{searchCommand.Title.Trim()}%";
                query = query
                    .Where(s => EF.Functions.Like(s.Title, textSearch) || EF.Functions.Like(s.Arranger.Name, textSearch))
                    .AsQueryable();
            }
            //returns only the songs shared with you that you have write permission on
            if (searchCommand?.SharedByUser == true)
            {
                query = query.Where(s => !s.SharedUsers.All(shared => shared.UserId != user.Id)).AsQueryable();
            }
            //if you only want to filter on groups 
            if( searchCommand.OrgId != null && searchCommand.GroupId == null)
            {
                query = query.Where(song =>
                song.SharedOrganisations.Any(org =>
                searchCommand.OrgId.Contains(org.OrganisationId))).AsQueryable();
            }
            //if you only want to filter on organisations
            else if (searchCommand.GroupId != null && searchCommand.OrgId == null)
            {
                query = query.Where(song =>
                song.SharedGroups.Any(group =>
                searchCommand.GroupId.Contains(group.GroupId))).AsQueryable();
            }

            //handles the case if you want one to se one organisation and a group not within that organisation
            else if( searchCommand.GroupId != null && searchCommand.OrgId != null)
            {
                query = query.Where(song => (
                song.SharedGroups.Any(group =>
                searchCommand.GroupId.Contains(group.GroupId)) ||

                song.SharedOrganisations.Any(org =>
                searchCommand.OrgId.Contains(org.OrganisationId)))
                ).AsQueryable();
            }

            if (searchCommand.ArrangerId != null)
            {
                query = query
                    .Where(s => s.ArrangerId == searchCommand.ArrangerId)
                    .AsQueryable();
            }

            if (searchCommand.OrderBy == "song")
            {
                query = searchCommand.OrderDescending ?
                    query.OrderBy(s => s.Title)
                    .ThenByDescending(s => s.UpdatedOn) :
                    query.OrderByDescending(s => s.Title)
                    .ThenByDescending(s => s.UpdatedOn);
            }
            else if (searchCommand.OrderBy == "user")
            {
                query = searchCommand.OrderDescending ?
                    query.OrderBy(s => s.Arranger.Name)
                    .ThenByDescending(s => s.UpdatedOn) :
                    query.OrderByDescending(s => s.Arranger.Name)
                    .ThenByDescending(s => s.UpdatedOn);
            }
            else
            {
                query = searchCommand.OrderDescending ? 
                    query.OrderByDescending(s => s.UpdatedOn) :
                    query.OrderBy(s => s.UpdatedOn);
            }

            if (searchCommand.Num != null)
            {
                query = query
                    .Take(searchCommand.Num.Value)
                    .AsQueryable();
            }

            var result = await query
                .ToArrayAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// Checks if the specified user har writing access to the specified song.
        /// </summary>
        /// <param name="song"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> HasWriteAccess(Song song, User user)
        {
            return song.ArrangerId == user.Id
                || await Context.SongSharedUser.AnyAsync(songSharedUser =>
                        songSharedUser.UserId == user.Id && songSharedUser.SongId == song.Id);
        }
    }
}
