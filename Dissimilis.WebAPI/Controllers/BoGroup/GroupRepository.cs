using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
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
                throw new NotFoundException($"Group with Id {groupId} not found");

            await Context.GroupUsers
                .Include(gu => gu.User)
                .Where(gu => gu.GroupId == groupId)
                .LoadAsync(cancellationToken);

            return group;
        }
        public async Task<Group[]> GetGroups(int? organisationId, string filterBy, User user, CancellationToken cancellationToken)
        {
            var query = Context.Groups
                .Include(x => x.Users)
                .AsQueryable();

            if (organisationId != null)
            {
                query = query.Where(g => g.OrganisationId == organisationId).AsQueryable();
            }
            query = query.FilterGroups(filterBy, user).AsQueryable();

            var result = await query
                .ToArrayAsync(cancellationToken);
            return result;
        }
    }
    public static class IQueryableExtension
    {
        public static IQueryable<Group> FilterGroups(this IQueryable<Group> groups, string filterBy, User user)
        {
            return filterBy switch
            {
                "ADMIN" => groups.Where(g => g.Users.Any(x => x.UserId == user.Id && x.Role == Role.Admin)).AsQueryable(),
                "MEMBER" => groups.Where(g => g.Users.Any(x => x.UserId == user.Id)).AsQueryable(),
                _ => groups
            };
        }
    } 
}
