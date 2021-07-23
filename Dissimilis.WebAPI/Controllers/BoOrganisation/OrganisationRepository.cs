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

namespace Dissimilis.WebAPI.Controllers.BoOrganisation
{
    public class OrganisationRepository
    {
        internal DissimilisDbContext Context;
        public OrganisationRepository(DissimilisDbContext context)
        {
            Context = context;
        }

        public async Task SaveOrganisationAsync(Organisation organisation, CancellationToken cancellationToken)
        {
            await Context.Organisations.AddAsync(organisation, cancellationToken);
            await UpdateAsync(cancellationToken);
        }

        public async Task SaveOrgUserAsync(OrganisationUser orgUser, CancellationToken cancellationToken)
        {
            await Context.OrganisationUsers.AddAsync(orgUser);
            await UpdateAsync(cancellationToken);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }


        public async Task<Organisation> GetOrganisationById(int organisationId, CancellationToken cancellationToken)
        {
            var organisation = await Context.Organisations
                .SingleOrDefaultAsync(o => o.Id == organisationId, cancellationToken);

            if (organisation == null)
            {
                throw new NotFoundException($"Organisation with Id {organisationId} not found");
            }

                await Context.OrganisationUsers
                    .Include(ou => ou.User)
                    .Where(ou => ou.OrganisationId == organisationId)
                    .LoadAsync(cancellationToken);

            return organisation;
        }

    public async Task<Organisation[]> GetOrganisationsAsync(string filterBy, User currentUser, CancellationToken cancellationToken)
    {
        return await Context.Organisations
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
            "ADMIN" => organisations.Where(o =>
            o.Users.Any(x => x.UserId == user.Id && x.Role == Role.Admin)).AsQueryable(),

            "GROUPADMIN" => organisations
            .Include(x => x.Groups)
            .ThenInclude(x => x.Users)
            .Where(o =>
            o.Groups.Any(g => g.Users.Any(u =>
            u.UserId == user.Id && u.Role == Role.Admin))).AsQueryable(),

            "MEMBER" => organisations.Where(o => o.Users.Any(x => x.UserId == user.Id)).AsQueryable(),
            _ => organisations,
        };
    }
}
}
