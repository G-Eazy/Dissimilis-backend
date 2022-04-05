using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using System;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.DbContext.Models.Song;
using System.Linq;

namespace Dissimilis.WebAPI.Services
{
    public class PermissionCheckerService : IPermissionCheckerService
    {
        private readonly DissimilisDbContext _dbContext;

        public PermissionCheckerService(DissimilisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Checks if a user has the privileges to perform desired operation on an organisation
        /// Sysadmins: All privileges
        /// Org-admins: All privileges except create/delete
        /// Other: No privileges
        /// </summary>
        /// <param name="organisation"></param>
        /// <param name="user"></param>
        /// <param name="op"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(Organisation organisation, User user, Operation op,
            CancellationToken cancellationToken)
        {
            return user.IsSystemAdmin
                    || (await IsGroupAdminInOrganisation(organisation.Id, user.Id, cancellationToken)
                        && op == Operation.Get)
                    || (await IsOrganisationAdmin(organisation.Id, user.Id, cancellationToken)
                        && op != Operation.Delete
                        && op != Operation.Create);
        }

        /// <summary>
        /// 
        /// Checks if a user has the privileges to perform desired operation on a group
        /// Sysadmins: All privileges
        /// Org admins: All privileges
        /// Group admins: All privileges except create/delete
        /// Other: No privileges
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="op"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(Group group, User user, Operation op, CancellationToken cancellationToken)
        {
            return user.IsSystemAdmin
                    || await IsOrganisationAdmin(group.OrganisationId, user.Id, cancellationToken)
                    || (await IsGroupAdmin(group.Id, user.Id, cancellationToken)
                        && op != Operation.Delete
                        && op != Operation.Create);
        }

        /// <summary>
        /// Method to check if user has any admin privileges wherever that may be.
        /// Used to let frontend regulate access to an admin view.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserAdminStatusDto> CheckUserAdminStatus(User user, CancellationToken cancellationToken)
        {
            bool isOrgAdmin = await _dbContext
                .OrganisationUsers
                .AnyAsync(ou =>
                    ou.UserId == user.Id
                    && ou.Role == Role.Admin);

            bool isGroupAdmin = await _dbContext
                .GroupUsers
                .AnyAsync(gu =>
                    gu.UserId == user.Id
                    && gu.Role == Role.Admin);

            return new UserAdminStatusDto() 
            {
                SystemAdmin = user.IsSystemAdmin,
                OrganisationAdmin = isOrgAdmin,
                GroupAdmin = isGroupAdmin
            };

        }

        /// <summary>
        /// Helper method to see if user is an organisation admin in specified organisation
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> IsOrganisationAdmin(int organisationId, int userId, CancellationToken cancellationToken)
        {
            return await _dbContext
                .OrganisationUsers
                .AnyAsync(ou =>
                    ou.UserId == userId
                    && ou.OrganisationId == organisationId
                    && ou.Role == Role.Admin
                    , cancellationToken: cancellationToken);
        }

        private async Task<bool> IsGroupAdminInOrganisation(int parentOrganisationId, int userId, CancellationToken cancellationToken)
        {
            return await _dbContext
                .Groups
                .Where(group => group.OrganisationId == parentOrganisationId)
                .AnyAsync(group =>
                    group.Users
                    .Any(groupUser => groupUser.UserId == userId && groupUser.Role == Role.Admin)
                    , cancellationToken);

        }

        /// <summary>
        /// Helper method to see if user is a group admin in specified group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> IsGroupAdmin(int groupId, int userId, CancellationToken cancellationToken)
        {
            return await _dbContext
                .GroupUsers
                .AnyAsync(gu =>
                    gu.UserId == userId
                    && gu.GroupId == groupId
                    && gu.Role == Role.Admin
                    , cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Checks if a user has the privileges to perform desired operation on a song
        /// Sysadmins: All privileges, not private songs
        /// Org admins: All privileges, not private songs
        /// Group admins: All privileges, not private songs
        /// Other: All priviliges on own songs and shared songs, read public
        /// </summary>
        /// <param name="song"></param>
        /// <param name="user"></param>
        /// <param name="op"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(Song song, User user, Operation op, CancellationToken cancellationToken)
        {
            if (op == Operation.Create) return true;
            if (op == Operation.Get && song.ProtectionLevel == ProtectionLevels.Public) return song.Deleted == null;
            if (op == Operation.Get || op == Operation.Modify || op == Operation.Invite || op == Operation.Kick)
            {
                return await CheckWriteAccess(song, user);
            }
            if (op == Operation.Delete || op == Operation.Restore)
            {
                return song.ArrangerId == user.Id
                    || user.IsSystemAdmin && song.ProtectionLevel == ProtectionLevels.Public;
            }

            return false;
        }

        /// <summary>
        /// Checks if a user has the privileges to perform desired operation on a user object
        /// Sysadmins: All privileges
        /// Self: All privileges
        /// Org admins: No privileges
        /// Group admins: No privileges
        /// Other: No privileges
        /// </summary>
        /// <param name="user"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public bool IsAdminUser(User currentUser)
        {
            return currentUser.IsSystemAdmin;
        }

        private async Task<bool> CheckWriteAccess(Song song, User user)
        {
            return user.IsSystemAdmin && song.ProtectionLevel == ProtectionLevels.Public 
                ||song.ArrangerId == user.Id
                ||await _dbContext.SongSharedUser.AnyAsync(songSharedUser =>
                songSharedUser.UserId == user.Id && songSharedUser.SongId == song.Id);
        }
    }
}
