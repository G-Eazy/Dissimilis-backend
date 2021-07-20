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
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Method to determine if user har permission to do desired operation with organisation object
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="user"></param>
        /// <param name="operation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(Organisation organisation, User user, string operation, CancellationToken cancellationToken)
        {
            if (user.IsSystemAdmin)
                return true;
            
            bool hasPermission = false;
            var orgAdmin = await Context.OrganisationUsers
                .SingleOrDefaultAsync(
                        ou =>
                        ou.UserId == user.Id
                        && ou.OrganisationId == organisation.Id
                        && ou.Role == Role.Admin
                     );
            if (orgAdmin != null)
                hasPermission = true;

            return hasPermission;
        }

        public async Task<Organisation> GetOrganisationById(int organisationId, CancellationToken cancellationToken)
        {
            var organisation = await Context.Organisations
                .SingleOrDefaultAsync(o => o.Id == organisationId, cancellationToken);

            if (organisation == null)
                throw new NotFoundException($"Organisation with Id {organisationId} not found");

            await Context.OrganisationUsers
                .Include(ou => ou.User)
                .Where(ou => ou.OrganisationId == organisationId)
                .LoadAsync(cancellationToken);

            return organisation;
        }
    }
}
