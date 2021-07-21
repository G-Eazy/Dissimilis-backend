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
                .SingleOrDefaultAsync(s => s.Id == songId, cancellationToken);

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
        public async Task<Song> GetSongWithTagsSharedUsers(int songId, CancellationToken cancellationToken)
        {
            var song = await Context.Songs
                .Include(s => s.SharedUsers)
                .ThenInclude(u => u.User)
                .Include(s => s.SharedOrganisations)
                .ThenInclude(o => o.Organisation)
                .Include(s => s.SharedGroups)
                .ThenInclude(g => g.Group)
                .AsSplitQuery()
                .SingleOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }
            return song;
        }


            public async Task<Song> GetFullSongById(int songId, CancellationToken cancellationToken)
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
                .AsSplitQuery()
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
        public async Task DeleteSongSharedUser(Song song, User user, SongSharedUser sharedSongUser, CancellationToken cancellationToken)
        {
            user.SongsShared.Remove(sharedSongUser);
            song.SharedUsers.Remove(sharedSongUser);
            Context.SongSharedUser.Remove(sharedSongUser);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteGroupTag(Song song, Group group, SongSharedGroup groupTag, CancellationToken cancellationToken)
        {
            group.SharedSongs.Remove(groupTag);
            song.SharedGroups.Remove(groupTag);
            Context.SongSharedGroups.Remove(groupTag);
            await Context.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteOrganisationTag(Song song, Organisation organisation, SongSharedOrganisation organisationTag, CancellationToken cancellationToken)
        {
            organisation.SharedSongs.Remove(organisationTag);
            song.SharedOrganisations.Remove(organisationTag);
            Context.SongSharedOrganisations.Remove(organisationTag);
            await Context.SaveChangesAsync(cancellationToken);
        }
        public async Task CreateAndAddSongShareUser(Song song, User user)
        {
            var songSharedUser = new SongSharedUser()
            {
                UserId = user.Id,
                SongId = song.Id
            };
            await Context.SongSharedUser.AddAsync(songSharedUser);
            song.SharedUsers.Add(songSharedUser);
            user.SongsShared.Add(songSharedUser);
            await Context.SaveChangesAsync();
        }
        public async Task CreateAndAddGroupTag(Song song, Group group)
        {
            var groupTag = new SongSharedGroup()
            {
                GroupId = group.Id,
                SongId = song.Id
            };
            await Context.SongSharedGroups.AddAsync(groupTag);
            song.SharedGroups.Add(groupTag);
            group.SharedSongs.Add(groupTag);
            await Context.SaveChangesAsync();
        }
        public async Task RemoveRedundantGroupTags(int[] groupIds, Song song, CancellationToken cancellationToken)
        {
            var groupTagsRemove = Context.SongSharedGroups
                .Where(x => x.SongId == song.Id && !groupIds.Contains(x.GroupId)).ToArray();
            foreach(var tag in groupTagsRemove)
            {
                var groupToRemove = Context.Groups.SingleOrDefault(x => x.Id == tag.GroupId);
                if(groupToRemove == null)
                {
                    throw new Exception($"Could not fint group with Id: {tag.GroupId}");
                };
                await DeleteGroupTag(song, groupToRemove, tag, cancellationToken);
            }
        }
        public async Task RemoveRedundantOrganisationTags(int[] organisationIds, Song song, CancellationToken cancellationToken)
        {
            var organisationTagsRemove = Context.SongSharedOrganisations
                .Where(x => x.SongId == song.Id && !organisationIds.Contains(x.OrganisationId)).ToArray();
            foreach (var tag in organisationTagsRemove)
            {
                var organisationToRemove = Context.Organisations.SingleOrDefault(x => x.Id == tag.OrganisationId);
                if (organisationToRemove == null)
                {
                    throw new Exception($"Could not fint organisation with Id: {tag.OrganisationId}");
                };
                await DeleteOrganisationTag(song, organisationToRemove, tag, cancellationToken);
            }
        }
        public async Task CreateAndAddOrganisationTag(Song song, Organisation organisation)
        {
            var organisationTag = new SongSharedOrganisation()
            {
                OrganisationId = organisation.Id,
                SongId = song.Id
            };
            await Context.SongSharedOrganisations.AddAsync(organisationTag);
            song.SharedOrganisations.Add(organisationTag);
            organisation.SharedSongs.Add(organisationTag);
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
