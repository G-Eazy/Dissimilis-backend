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

        public async Task<OrganisationUser> GetOrganisationUserAsync(int organisationId, int userId, CancellationToken cancellationToken)
        {
            return await Context.OrganisationUsers
                .SingleOrDefaultAsync(orgUser =>
                    orgUser.UserId == userId && orgUser.OrganisationId == organisationId);
        }

        public async Task<OrganisationUser> AddUserToOrganisationAsync(int organisationId, int userId, Role role, CancellationToken cancellationToken)
        {
            var orgUserAdded = await Context.OrganisationUsers
                .AddAsync(
                    new OrganisationUser()
                    {
                        OrganisationId = organisationId,
                        UserId = userId,
                        Role = role,
                    }, cancellationToken);

            return orgUserAdded.Entity;
        }
    }
}
