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

        public async Task<Group[]> GetGroups(bool OnlyMyGroups, bool OnlyAdmins, int? OrganisationId, User user, CancellationToken cancellationToken)
        {
            var query = context.Groups.AsQueryable();
            if (OnlyMyGroups || OnlyAdmins)
            {
                var userGroups = user.GetAllGroupIds();
                query = query.Where(g => userGroups.Contains(g.Id)).AsQueryable();
                if (OnlyAdmins)
                {
                    query = query.Where(g => g.Users.Any(u => u.UserId == user.Id && u.Role == Role.Admin));
                }
            }
            else if(OrganisationId != null)
            {
                query = query.Where(g => g.OrganisationId == OrganisationId);
            }
            var result = await query
                .ToArrayAsync(cancellationToken);
            return result;
        }
    }
}
