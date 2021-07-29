using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Extensions.Models;
using System;
using Dissimilis.WebAPI.Services;
using System.Collections.Generic;


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

            await Context.SongSnapshots
                .Include(s => s.Song)
                .Where(s => s.SongId == songId)
                .LoadAsync(cancellationToken);

            return song;
        }
        public async Task<Song> GetSongWithTagsSharedUsers(int songId, CancellationToken cancellationToken)
        {
            var song = await Context.Songs
                .SingleOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            await Context.SongSharedUser
                .Include(su => su.User)
                .Where(su => su.SongId == song.Id)
                .LoadAsync(cancellationToken);

            await Context.SongGroupTags
                .Include(su => su.Group)
                .Where(su => su.SongId == song.Id)
                .LoadAsync(cancellationToken);

            await Context.SongOrganisationTags
                .Include(su => su.Organisation)
                .Where(su => su.SongId == song.Id)
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

            await Context.SongSnapshots
                .Include(s => s.Song)
                .Where(s => s.SongId == songId)
                .LoadAsync(cancellationToken);

            return song;
        }

        public async Task SaveAsync(Song song, CancellationToken cancellationToken)
        {
            await Context.Songs.AddAsync(song, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// </summary>
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
        /// </summary>
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

        public async Task<List<Song>> GetSongSearchList(User user, SearchQueryDto searchCommand, CancellationToken cancellationToken)
        {
            return await Context.Songs
                .Include(song => song.Arranger)
                .Include(song => song.SharedUsers)
                .Include(song => song.GroupTags)
                .Include(song => song.OrganisationTags)
                .Where(song => song.Deleted == null)
                .Where(SongExtension.ReadAccessToSong(user))
                .AsSplitQuery()
                .AsQueryable()
                .FilterQueryable(user, searchCommand.Title, searchCommand.ArrangerId, searchCommand.IncludedOrganisationIdArray, searchCommand.IncludedGroupIdArray, searchCommand.IncludeSharedWithUser)
                .OrderQueryable(searchCommand.OrderBy, searchCommand.OrderDescending)
                .Take(searchCommand.MaxNumberOfSongs)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteSongSharedUser(SongSharedUser sharedSongUser, CancellationToken cancellationToken)
        {
            Context.SongSharedUser.Remove(sharedSongUser);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteGroupTag(SongGroupTag groupTag, CancellationToken cancellationToken)
        {
            Context.SongGroupTags.Remove(groupTag);
            await Context.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteOrganisationTag(SongOrganisationTag organisationTag, CancellationToken cancellationToken)
        {
            Context.SongOrganisationTags.Remove(organisationTag);
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
            await Context.SaveChangesAsync();
            
        }
        public async Task CreateAndAddGroupTag(Song song, Group group)
        {
            var groupTag = new SongGroupTag()
            {
                GroupId = group.Id,
                SongId = song.Id
            };
            await Context.SongGroupTags.AddAsync(groupTag);
            await Context.SaveChangesAsync();
        }
        public async Task RemoveRedundantGroupTags(int[] groupIds, Song song, CancellationToken cancellationToken)
        {
            var groupTagsRemove = Context.SongGroupTags
                .Where(x => x.SongId == song.Id && !groupIds.Contains(x.GroupId)).ToArray();
            foreach(var tag in groupTagsRemove)
            {
                await DeleteGroupTag(tag, cancellationToken);
            }
        }
        public async Task RemoveRedundantOrganisationTags(int[] organisationIds, Song song, CancellationToken cancellationToken)
        {
            var organisationTagsRemove = Context.SongOrganisationTags
                .Where(x => x.SongId == song.Id && !organisationIds.Contains(x.OrganisationId)).ToArray();
            foreach (var tag in organisationTagsRemove)
            {
                await DeleteOrganisationTag(tag, cancellationToken);
            }
        }
        public async Task CreateAndAddOrganisationTag(Song song, Organisation organisation)
        {
            var organisationTag = new SongOrganisationTag()
            {
                OrganisationId = organisation.Id,
                SongId = song.Id
            };
            await Context.SongOrganisationTags.AddAsync(organisationTag);
            await Context.SaveChangesAsync();
        }
        public async Task<SongSharedUser> GetSongSharedUser(int songId, int userId)
        {
            return await Context.SongSharedUser.SingleOrDefaultAsync(x => x.SongId == songId && x.UserId == userId);
        }

        public async Task<SongGroupTag> GetSongSharedGroup(int songId, int groupId)
        {
            return await Context.SongGroupTags.SingleOrDefaultAsync(x => x.SongId == songId && x.GroupId == groupId);
        }
        public async Task<SongOrganisationTag> GetSongSharedOrganisation(int songId, int organisationId)
        {
            return await Context.SongOrganisationTags.SingleOrDefaultAsync(x => x.SongId == songId && x.OrganisationId == organisationId);
        }
    }

    public static class IQueryableExtension
    {
        public static IQueryable<Song> FilterQueryable(this IQueryable<Song> songs, User currentUser, string searchText, int? arrangerId, int[] includedOrganisationIdArray, int[] includedGroupIdArray, bool includeSharedWithUser)
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
                            (!includeSharedWithUser && includedGroupIdArray.Length == 0 && includedOrganisationIdArray.Length == 0)
                        ||
                            (includeSharedWithUser && song.SharedUsers.Any(sharedSong => sharedSong.UserId == currentUser.Id))
                            || song.OrganisationTags.Any(organisation => includedOrganisationIdArray.Contains(organisation.OrganisationId))
                            || song.GroupTags.Any(group => includedGroupIdArray.Contains(group.GroupId))
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
