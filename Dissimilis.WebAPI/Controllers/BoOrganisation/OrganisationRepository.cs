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

        public async Task<Organisation[]> GetOrganisations(string filterBy, User currentUser, CancellationToken cancellationToken)
        {
            return await context.Organisations
                            .Include(x => x.Users)
                            .AsQueryable().FilterOrganisations(filterBy, currentUser).ToArrayAsync(cancellationToken);
        }
    }
    public static class IQueryableExtension
    {
        public static IQueryable<Organisation> FilterOrganisations(this IQueryable<Organisation> organisations, string filterBy, User user)
        {
            return filterBy switch
            {
                "Admin" => organisations.Where(o => 
                o.Users.Any(x => x.UserId == user.Id && x.Role == Role.Admin)).AsQueryable(),

                "GroupAdmin" => organisations
                .Include(x => x.Groups)
                .ThenInclude(x => x.Users)
                .Where(o =>
                o.Groups.Any(g => g.Users.Any(u =>
                u.UserId == user.Id && u.Role == Role.Admin))).AsQueryable(),

                "Member" => organisations.Where(o => o.Users.Any(x => x.UserId == user.Id)).AsQueryable(),
                _ => organisations,
            };
        }
    }
}
