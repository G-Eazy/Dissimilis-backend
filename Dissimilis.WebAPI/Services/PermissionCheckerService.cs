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
            return user.IsSystemAdmin
                    || (await IsOrganisationAdmin(organisation.Id, user.Id, cancellationToken)
                        && op != Operation.Delete
                        && op != Operation.Create);
        }

        /// <summary>
        /// Helper method to fetch a user from an organisations admins if specified user is an admin in said org.
        /// Returns null if user is not.
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

        /// <summary>
        /// Helper method to fetch a user from a groups admins if specified user is an admin in said group.
        /// Returns null if user is not.
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
    }
}
