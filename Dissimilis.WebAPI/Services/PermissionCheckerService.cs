using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using System;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Services
{
    public class PermissionCheckerService : _IPermissionCheckerService
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
            if (user.IsSystemAdmin)
                return true;

            bool isAllowed = false;

            OrganisationUser orgAdmin = null;
            Console.WriteLine(organisation.Id);
            if (organisation.Id != 0)

                orgAdmin = await GetOrgAdminIfExists(organisation, user, cancellationToken);

            if (orgAdmin != null && (op == Operation.Create || op != Operation.Delete))
            {
                isAllowed = true;
            }

            return isAllowed;
        }

        /// <summary>
        /// Helper method to fetch a user from an organisations admins if specified user is an admin in said org.
        /// Returns null if user is not.
        /// </summary>
        /// <param name="organisation"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<OrganisationUser> GetOrgAdminIfExists(Organisation organisation, User user,
            CancellationToken cancellationToken)
        { 
            var orgUser = await _dbContext.OrganisationUsers
                .SingleOrDefaultAsync(ou => 
                ou.UserId == user.Id && ou.OrganisationId == organisation.Id 
                && ou.Role == Role.Admin, cancellationToken);
            return orgUser;
        }

        /// <summary>
        /// Helper method to fetch a user from a groups admins if specified user is an admin in said group.
        /// Returns null if user is not.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<GroupUser> GetGroupAdminIfExists(Group group, User user, CancellationToken cancellationToken)
        {
            var groupUser = await _dbContext.GroupUsers
                .SingleOrDefaultAsync(gu => gu.UserId == user.Id && gu.GroupId == group.Id && gu.Role == Role.Admin, cancellationToken);
            return groupUser;
        }

        /// <summary>
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
            if (user.IsSystemAdmin)
                return true;

            var parentOrganisation = await _dbContext.Organisations
                .SingleOrDefaultAsync(o => o.Id == group.OrganisationId, cancellationToken);
            var orgAdmin = await GetOrgAdminIfExists(parentOrganisation, user, cancellationToken);
            if (orgAdmin != null)
                return true;

            bool isAllowed = false;
            GroupUser groupAdmin = null;
            if (group.Id != 0)
                groupAdmin = await GetGroupAdminIfExists(group, user, cancellationToken);

            if(groupAdmin != null && (op == Operation.Create || op != Operation.Delete))
            {
                isAllowed = true;
            }

            return isAllowed;
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
            if (op == Operation.Get && song.ProtectionLevel == ProtectionLevels.Public) return true;
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
        private async Task<bool> CheckWriteAccess(Song song, User user)
        {
            return user.IsSystemAdmin && song.ProtectionLevel == ProtectionLevels.Public 
                ||song.ArrangerId == user.Id
                ||await _dbContext.SongSharedUser.AnyAsync(songSharedUser =>
                songSharedUser.UserId == user.Id && songSharedUser.SongId == song.Id);
        }
    }
}
