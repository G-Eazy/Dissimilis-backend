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
    public class PermissionChecker
    {
        private readonly DissimilisDbContext _dbContext;

        public PermissionChecker(DissimilisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

        private async Task<OrganisationUser> GetOrgAdminIfExists(Organisation organisation, User user, CancellationToken cancellationToken)
        { 
            var orgUser = await _dbContext.OrganisationUsers
                .SingleOrDefaultAsync(ou => ou.UserId == user.Id && ou.OrganisationId == organisation.Id && ou.Role == Role.Admin);
            return orgUser;
        }

        private async Task<GroupUser> GetGroupAdminIfExists(Group group, User user, CancellationToken cancellationToken)
        {
            var groupUser = await _dbContext.GroupUsers
                .SingleOrDefaultAsync(gu => gu.UserId == user.Id && gu.GroupId == group.Id && gu.Role == Role.Admin);
            return groupUser;
        }

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

        public bool CheckCreateOrganisationPermission(User user)
        {
            return user.IsSystemAdmin;
        }
    }
}
