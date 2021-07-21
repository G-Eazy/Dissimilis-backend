using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Dissimilis.DbContext.Models;
using System;


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
                .Where(s => (s.CreatedById == userId || s.ArrangerId == userId) && s.Deleted == null)
                .ToArrayAsync(cancellationToken);
            
            return songs;
        }

        public async Task SaveAsync(Song song, CancellationToken cancellationToken)
        {
            await Context.Songs.AddAsync(song, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Song[]> GetMyDeletedSongs(User user, CancellationToken cancellationToken)
        {

            // RemoveDeletedSongOlderThanDays should be made into a background job at a later date!!!
            await RemoveDeletedSongsOlderThanDays(user, 30, cancellationToken);
            var songs = Context.Songs
                .Include(s => s.Arranger)
                .Include(s => s.CreatedBy)
                .Include(s => s.UpdatedBy)
                .Where(s => s.ArrangerId == user.Id && s.Deleted != null)
                .ToArray();

            if (songs == null || songs.ToArray().Length == 0)
            {
                throw new NotFoundException($"User has no deleted songs from the last 30 days");
            }

            return songs;
        }

        /// <summary>
        /// Method to be used to find songs that are marked as deleted, should not be used to look up songs to edit.
        /// 
        /// ******************************************************************
        /// 
        ///     THIS METHOD SHOULD BE MADE INTO A BACKGROUND JOB LATER
        /// 
        /// ******************************************************************
        /// 
        public async Task RemoveDeletedSongsOlderThanDays(User user, int nDays, CancellationToken cancellationToken)
        {
            var oldestAllowedDate = DateTimeOffset.Now.AddDays(-nDays);

            var songs = Context.Songs
                .Where(s => s.ArrangerId == user.Id && s.Deleted < oldestAllowedDate)
                .ToArray();

            foreach (var song in songs)
                Context.Songs.Remove(song);
            
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

        public async Task DeleteSong(User user, Song song, CancellationToken cancellationToken)
        {
            song.Deleted = DateTimeOffset.Now;

            /* 
             * ******************************************************************
            
                Add RemoveDeletedSongsOlderThanDays() as background job later!!!

            * *******************************************************************
            */
            await RemoveDeletedSongsOlderThanDays(user, 30, cancellationToken);


            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task RestoreSong(Song song, CancellationToken cancellationToken)
        {
            song.Deleted = null;
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Song[]> GetSongSearchList(User user, SearchQueryDto searchCommand, CancellationToken cancellationToken)
        {

            var query = Context.Songs
                .Include(s => s.Arranger)
                .Where(s => s.Deleted == null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchCommand.Title))
            {
                var textSearch = $"%{searchCommand.Title.Trim()}%";
                query = query
                    .Where(s => EF.Functions.Like(s.Title, textSearch) || EF.Functions.Like(s.Arranger.Name, textSearch))
                    .AsQueryable();
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

            if (searchCommand.MaxNumberOfSongs != 0)
            {
                query = query
                    .Take(searchCommand.MaxNumberOfSongs)
                    .AsQueryable();
            }

            var result = await query
                .ToArrayAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// Checks if the specified user has writing access to the specified song.
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

    public static class IQueryableExtension
    {
        public static IQueryable<Song> FilterQueryable(this IQueryable<Song> songs, User currentUser, string searchText, int? arrangerId, int[] includedOrganisationIdArray, int[] includedGroupIdArray, bool includeSharedWithUser, bool includeAll)
        {
            return songs
                .Where(song =>
                    (   (searchText == null
                        || EF.Functions.Like(song.Title, $"%{searchText.Trim()}%")
                        || EF.Functions.Like(song.Arranger.Name, $"%{searchText.Trim()}%"))
                        && (arrangerId == null || song.ArrangerId == arrangerId)
                    )
                    &&
                    (
                        includeAll
                        ||
                        (
                            (includeSharedWithUser && song.SharedUsers.Any(sharedSong => sharedSong.UserId == currentUser.Id))
                            || song.SharedOrganisations.Any(organisation => includedOrganisationIdArray.Contains(organisation.OrganisationId))
                            || song.SharedGroups.Any(group => includedGroupIdArray.Contains(group.GroupId))
                        )
                    )
                    );
        }

        public static IQueryable<Song> OrderQueryable(this IQueryable<Song> songs, string fieldToOrderBy, bool isDescendingSort)
        {
            return fieldToOrderBy switch
            {
                "song" => isDescendingSort
                                       ? songs.OrderByDescending(song => song.Title).ThenByDescending(song => song.UpdatedOn)
                                       : songs.OrderBy(song => song.Title).ThenBy(song => song.UpdatedOn),
                "user" => isDescendingSort
                                        ? songs.OrderByDescending(song => song.Arranger.Name).ThenByDescending(song => song.UpdatedOn)
                                        : songs.OrderBy(song => song.Arranger.Name).ThenBy(song => song.UpdatedOn),
                _ => isDescendingSort
                                  ? songs.OrderByDescending(song => song.UpdatedOn)
                                  : songs.OrderBy(song => song.UpdatedOn),
            };
        }
    }
}
