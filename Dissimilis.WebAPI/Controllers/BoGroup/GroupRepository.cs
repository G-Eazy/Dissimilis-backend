using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    public class GroupRepository
    {
        internal readonly DissimilisDbContext context;

        public GroupRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        public async Task<Group> GetGroupById(int groupId, CancellationToken cancellationToken)
        {
            var group = await context.Groups.SingleOrDefaultAsync(g => g.Id == groupId, cancellationToken);

            if (group == null)
            {
                throw new NotFoundException($"Group with Id {groupId} not found");
            }
            return group;
        }

        public async Task<Group[]> GetGroups(int? organisationId, string filterBy, User user, CancellationToken cancellationToken)
        {
            var query = context.Groups
                .Include(x => x.Users)
                .AsQueryable();

            if (organisationId != null)
            {
                query.Where(g => g.OrganisationId == organisationId).AsQueryable();
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
                "Admin" => groups.Where(g => g.Users.Any(x => x.UserId == user.Id && x.Role == Role.Admin)).AsQueryable(),
                "Member" => groups.Where(g => g.Users.Any(x => x.UserId == user.Id)).AsQueryable(),
                _ => groups
            };
        }
    } 
}