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
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="operation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(Group group, User user, string operation, CancellationToken cancellationToken)
        {
            if (user.IsSystemAdmin)
                return true;

            bool hasPermission = false;

            var orgAdmin = await Context.OrganisationUsers
                    .SingleOrDefaultAsync(
                        ou =>
                        ou.UserId == user.Id
                        && ou.OrganisationId == group.OrganisationId
                        && ou.Role == Role.Admin
                     );

            switch(operation)
            {
                case "add": 
                    if(orgAdmin != null)
                        hasPermission = true;
                    break;

                case "modify":
                    var groupAdmin = await Context.GroupUsers
                        .SingleOrDefaultAsync(
                            gu =>
                            gu.UserId == user.Id
                            && gu.GroupId == group.Id
                            && gu.Role == Role.Admin
                        );
                    if (groupAdmin != null)
                        hasPermission = true;
                    break;

                default:
                    break;
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
