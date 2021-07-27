using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    public class GroupRepository
    {
        internal readonly DissimilisDbContext Context;
        
        public GroupRepository(DissimilisDbContext context)
        {
            Context = context;
        }

        public async Task SaveGroupAsync(Group group, CancellationToken cancellationToken)
        {
            await Context.Groups.AddAsync(group, cancellationToken);
            await UpdateAsync(cancellationToken);
        }

        public async Task SaveGroupUserAsync(GroupUser groupUser, CancellationToken cancellationToken)
        {
            await Context.GroupUsers.AddAsync(groupUser, cancellationToken);
            await UpdateAsync(cancellationToken);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Group[]> GetGroupsAsync(int? organisationId, string filterBy, User currentUser, CancellationToken cancellationToken)
        {
            var query = Context.Groups
                .Include(x => x.Users)
                .Include(x => x.Organisation)
                .AsQueryable();
            if (organisationId != null)
            {
                query = query.Where(g => g.OrganisationId == organisationId).AsQueryable();
            }
            query = query.FilterGroups(filterBy, currentUser).AsQueryable();

            var result = await query
                .ToArrayAsync(cancellationToken);
            return result;
        }


        public async Task<Group> GetGroupByIdAsync(int groupId, CancellationToken cancellationToken)
        {
            var group = await Context.Groups
                .Include(g => g.Organisation)
                .SingleOrDefaultAsync(g => g.Id == groupId, cancellationToken);

            if (group == null)
            {
                throw new NotFoundException($"Group with Id {groupId} not found");
            }

                await Context.GroupUsers
                    .Include(gu => gu.User)
                    .Where(gu => gu.GroupId == groupId)
                    .LoadAsync(cancellationToken);

            return group;
        }

        internal async Task<GroupUser> GetGroupUserAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            return await Context.GroupUsers.FindAsync(userId, groupId);
        }

        internal async Task<bool> CheckUserAdminAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            var groupUser = await GetGroupUserAsync(userId, groupId, cancellationToken);
            return groupUser?.Role == Role.Admin;
        }

        internal async Task<bool> IsUserLastAdmin(int userId, int groupId, CancellationToken cancellationToken)
        {
            return await CheckUserAdminAsync(userId, groupId, cancellationToken)
                    && !CheckIfAnotherGroupAdminExists(userId, groupId);
        }

        internal bool CheckIfAnotherGroupAdminExists(int userId, int groupId)
        {
            return Context.GroupUsers.Any(groupUser =>
                groupUser.GroupId == groupId
                && groupUser.UserId != userId
                && groupUser.Role == Role.Admin);
        }

        internal async Task<GroupUser> AddUserToGroupAsync(int userId, int groupId, Role role, CancellationToken cancellationToken)
        {
            var groupUser = await Context.GroupUsers.AddAsync(new GroupUser() { GroupId = groupId, UserId = userId, Role = role }, cancellationToken);

            return groupUser.Entity;
        }

        internal async Task<GroupUser> RemoveUserFromGroupAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            var groupUserToDelete = await GetGroupUserAsync(userId, groupId, cancellationToken);
            Context.GroupUsers.Remove(groupUserToDelete);
            return groupUserToDelete;
        }

        internal async Task<GroupUser> ChangeUserRoleAsync(int userId, int groupId, Role newRole, CancellationToken cancellationToken)
        {
            var groupUser = await GetGroupUserAsync(userId, groupId, cancellationToken);
            if (groupUser == null) throw new NotFoundException($"User with id {userId} is not a in the group with id {groupId}");

            groupUser.Role = newRole;
            await UpdateAsync(cancellationToken);

            return groupUser;
        }
    }
    public static class IQueryableExtension
    {
        public static IQueryable<Group> FilterGroups(this IQueryable<Group> groups, string filterBy, User user)
        {
            return filterBy switch
            {
                "ADMIN" => groups.Where(o =>
                o.Users.Any(x => x.UserId == user.Id && x.Role == Role.Admin)).AsQueryable(),

                "MEMBER" => groups.Where(o => o.Users.Any(x => x.UserId == user.Id)).AsQueryable(),
                _ => groups,
            };
        }
    }
}
