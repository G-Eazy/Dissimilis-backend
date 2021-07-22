using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Controllers.BoOrganisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using Dissimilis.Configuration;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.AspNetCore.Http;
using Dissimilis.DbContext.Models.Enums;
using System.Threading;
using Microsoft.EntityFrameworkCore;

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
        /// 
        /// Checks if a user has the privileges to perform desired operation on an organisation
        /// Sysadmins: All privileges
        /// Org-admins: All privileges except create/delete
        /// Other: No privileges
        /// 
        /// </summary>
        /// <param name="organisation"></param>
        /// <param name="user"></param>
        /// <param name="op"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(Organisation organisation, User user, Operation op, CancellationToken cancellationToken)
        {
            if (user.IsSystemAdmin)
                return true;

            bool isAllowed = false;

            OrganisationUser orgAdmin = null;
            if (organisation.Id != null)
                orgAdmin = await GetOrgAdminIfExists(organisation, user, cancellationToken);

            if (orgAdmin != null)
            {
                if (op != Operation.Create && op != Operation.Delete)
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
        private async Task<OrganisationUser> GetOrgAdminIfExists(Organisation organisation, User user, CancellationToken cancellationToken)
        { 
            var orgUser = await _dbContext.OrganisationUsers
                .SingleOrDefaultAsync(ou => ou.UserId == user.Id && ou.OrganisationId == organisation.Id && ou.Role == Role.Admin);
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
                .SingleOrDefaultAsync(gu => gu.UserId == user.Id && gu.GroupId == group.Id && gu.Role == Role.Admin);
            return groupUser;
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
            if (user.IsSystemAdmin)
                return true;

            var parentOrganisation = await _dbContext.Organisations
                .SingleOrDefaultAsync(o => o.Id == group.OrganisationId);
            var orgAdmin = await GetOrgAdminIfExists(parentOrganisation, user, cancellationToken);
            if (orgAdmin != null)
                return true;

            bool isAllowed = false;
            GroupUser groupAdmin = null;
            if (group.Id != null)
                groupAdmin = await GetGroupAdminIfExists(group, user, cancellationToken);

            if(groupAdmin != null)
            {
                if (op == Operation.Create || op != Operation.Delete)
                    isAllowed = true;
            }

            return isAllowed;
        }

        /// <summary>
        /// Might be removed later. Could be useful to explicitly check for create permissions on an org.
        /// Might be unnecessary...
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckCreateOrganisationPermission(User user)
        {
            return user.IsSystemAdmin;
        }
    }
}
