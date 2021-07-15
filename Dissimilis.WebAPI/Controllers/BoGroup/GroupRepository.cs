using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    public class OrganisationRepository
    {
        internal readonly DissimilisDbContext context;

        public OrganisationRepository(DissimilisDbContext context)
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
    }
}
