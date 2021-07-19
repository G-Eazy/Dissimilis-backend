using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Extensions.Models;
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
                .Where(s => s.ArrangerId == userId)
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
                .SingleOrDefaultAsync(s => s.Id == songId, cancellationToken);

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
            return await Context.Songs
                .Include(song => song.Arranger)
                .Include(song => song.SharedUsers)
                .Include(song => song.SharedGroups)
                .Include(song => song.SharedOrganisations)
                .AsQueryable()
                .Where(SongExtension.ReadAccessToSong(user))
                .FilterQueryable(user, searchCommand.Title, searchCommand.ArrangerId, searchCommand.IncludedOrganisationIdArray, searchCommand.IncludedGroupIdArray, searchCommand.IncludeSharedWithUser, searchCommand.IncludeAll)
                .OrderQueryable(searchCommand.OrderBy, searchCommand.OrderDescending)
                .Take(searchCommand.MaxNumberOfSongs)
                .ToArrayAsync(cancellationToken);
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
        public async Task CreateSongShareUser(Song song, User user)
        {
            var songSharedUser = new SongSharedUser()
            {
                UserId = user.Id,
                SongId = song.Id
            };
            await Context.SongSharedUser.AddAsync(songSharedUser);
            await Context.SaveChangesAsync();
        }
        public async Task<SongSharedUser> GetSongSharedUser(int songId, int userId)
        {
            return await Context.SongSharedUser.SingleOrDefaultAsync(x => x.SongId == songId && x.UserId == userId);
        }

        public async Task<SongSharedGroup> GetSongSharedGroup(int songId, int groupId)
        {
            return await Context.SongSharedGroups.SingleOrDefaultAsync(x => x.SongId == songId && x.GroupId == groupId);
        }
        public async Task<SongSharedOrganisation> GetSongSharedOrganisation(int songId, int organisationId)
        {
            return await Context.SongSharedOrganisations.SingleOrDefaultAsync(x => x.SongId == songId && x.OrganisationId == organisationId);
        }
    }

    public static class IQueryableExtension
    {
        public static IQueryable<Song> FilterQueryable(this IQueryable<Song> songs, User currentUser, string searchText, int? arrangerId, int[] includedOrganisationIdArray, int[] includedGroupIdArray, bool includeSharedWithUser, bool includeAll)
        {
            return songs
                .Where(song =>
                    (   (EF.Functions.Like(song.Title, $"%{searchText.Trim()}%")
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
