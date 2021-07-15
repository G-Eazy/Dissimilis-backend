using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation
{
    public class OrganisationRepository
    {
        internal readonly DissimilisDbContext context;

        public OrganisationRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        public async Task<Organisation> GetOrganisationById(int organisationId, CancellationToken cancellationToken)
        {
            var organisation = await context.Organisations.SingleOrDefaultAsync(o => o.Id == organisationId, cancellationToken);

            if (organisation == null)
            {
                throw new NotFoundException($"Organisation with Id {organisationId} not found");
            }
            return organisation;
        }
    }
}
