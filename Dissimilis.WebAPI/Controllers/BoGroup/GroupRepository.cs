using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    public class GroupRepository
    {
        internal DissimilisDbContext Context;
        public GroupRepository(DissimilisDbContext context)
        {
            Context = context;
        }

        public async Task SaveGroupAsync(Group group, CancellationToken cancellationToken)
        {
            await Context.Groups.AddAsync(group, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Method to determine if user har permission to do desired operation with group object
        /// Can be extended later to fit operations other than "add"
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="user"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(int organisationId, User user, string operation, CancellationToken cancellationToken)
        {
            if (user.IsSystemAdmin) 
                return true;

            bool hasPermission = false;
            if (operation == "add")
            {
                var res = await Context.OrganisationUsers
                    .SingleOrDefaultAsync(ou => ou.UserId == user.Id && ou.OrganisationId == organisationId);

                if (res != null)
                {
                    if (res.Role == Role.Admin)
                        hasPermission = true;
                }
            }
            return hasPermission;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Group> GetGroupById(int groupId, CancellationToken cancellationToken)
        {
            var group = await Context.Groups
                .Include(g => g.Organisation)
                .SingleOrDefaultAsync(g => g.Id == groupId, cancellationToken);

            if (group == null)
                throw new NotFoundException($"Organisation with Id {groupId} not found");

            await Context.GroupUsers
                .Include(gu => gu.User)
                .Where(gu => gu.GroupId == groupId)
                .LoadAsync(cancellationToken);

            return group;
        }
    }
}
