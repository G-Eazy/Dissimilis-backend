using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
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
        internal async Task<GroupUser> GetGroupUserAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            return await Context.GroupUsers.FindAsync(userId, groupId);
        }

        internal async Task<bool> CheckUserAdminAsync(int userId, int groupId, CancellationToken cancellationToken)
        {
            var groupUser = await GetGroupUserAsync(userId, groupId, cancellationToken);
            return groupUser?.Role == Role.Admin;
        }

        internal GroupUser FindNextAdmin()
        {
            return Context.GroupUsers.FirstOrDefault(groupUser => groupUser.Role == Role.Admin);
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
    }
}
