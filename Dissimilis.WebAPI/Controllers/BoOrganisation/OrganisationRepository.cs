using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
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


        public async Task<Organisation> GetOrganisationByIdAsync(int organisationId, CancellationToken cancellationToken)
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
                            .AsQueryable()
                            .FilterOrganisations(filterBy, currentUser)
                            .ToArrayAsync(cancellationToken);
        }
        public async Task<OrganisationUser> GetOrganisationUserAsync(int organisationId, int userId, CancellationToken cancellationToken)
        {
            return await Context.OrganisationUsers
                .SingleOrDefaultAsync(orgUser =>
                    orgUser.UserId == userId && orgUser.OrganisationId == organisationId);
        }

        internal async Task<OrganisationUser> AddUserToOrganisationAsync(int organisationId, int userId, Role role, CancellationToken cancellationToken)
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

        internal async Task<bool> CheckUserAdminAsync(int organisationId, int userId, CancellationToken cancellationToken)
        {
            var orgUser = await GetOrganisationUserAsync(organisationId, userId, cancellationToken);
            return orgUser?.Role == Role.Admin;
        }

        internal async Task<bool> CheckIfAnotherOrganisationAdminExists(int organisationId, int userId, CancellationToken cancellationToken)
        {
            return await Context.OrganisationUsers
                .AnyAsync(orgUser =>
                    orgUser.OrganisationId == organisationId
                    && orgUser.UserId != userId
                    && orgUser.Role == Role.Admin,
                    cancellationToken);
        }

        internal async Task<bool> IsUserLastAdmin(int organisationId, int userId, CancellationToken cancellationToken)
        {
            return await CheckUserAdminAsync(organisationId, userId, cancellationToken)
                && !await CheckIfAnotherOrganisationAdminExists(organisationId, userId, cancellationToken);
        }

        internal async Task<OrganisationUser> RemoveUserFromOrganisationAsync(int organisationId, int userId, CancellationToken cancellationToken)
        {
            var orgUserToRemove = await Context.OrganisationUsers
                .SingleOrDefaultAsync(orgUser =>
                    orgUser.OrganisationId == organisationId
                    && orgUser.UserId == userId);
            var orgUserRemoved = Context.Remove(orgUserToRemove);
            return orgUserRemoved.Entity;
        }
        internal async Task<OrganisationUser> ChangeUserRoleAsync(int userId, int organisationId, Role newRole, CancellationToken cancellationToken)
        {
            var organisationUser = await GetOrganisationUserAsync(organisationId, userId, cancellationToken);
            if (organisationUser == null) throw new NotFoundException($"User with id {userId} is not a in the organisation with id {organisationId}");

            organisationUser.Role = newRole;
            await UpdateAsync(cancellationToken);

            return organisationUser;
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
