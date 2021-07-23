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
        internal readonly DissimilisDbContext Context;
        
        public GroupRepository(DissimilisDbContext context)
        {
            Context = context;
        }
        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.Groups.AddAsync(group, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        internal async Task<GroupUser> GetGroupUserAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            return await Context.GroupUsers.FindAsync(userId, groupId, cancellationToken);
        }

        internal async Task<bool> CheckUserAdminAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            var groupUser = await GetGroupUserAsync(userId, groupId, cancellationToken);
            return groupUser?.Role == Role.Admin;
        }

        internal GroupUser FindOtherAdmin(User adminUser)
        {
            return Context.GroupUsers.FirstOrDefault(groupUser => groupUser.UserId == adminUser.Id && groupUser.Role == Role.Admin);
        }

        internal async Task<GroupUser> AddUserToGroupAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            var groupUser = await Context.GroupUsers.AddAsync(new GroupUser() { GroupId = groupId, UserId = userId, Role = Role.Member }, cancellationToken);
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
            return groupUser;
        }
    }
}
