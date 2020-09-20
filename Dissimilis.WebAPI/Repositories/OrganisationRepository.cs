using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Repositories
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private DissimilisDbContext _context;
        public OrganisationRepository(DissimilisDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Find or create an organisation depending on the organisaion metadata
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Organisation> CreateOrFindOrganisationAsync(OrganizationMetadata metadata, uint userId)
        {
            Organisation organisation;
            //Check if metadata is null
            if(metadata is null)
            {   
                //if null, get default organisation
                organisation = await GetOrganisationByNameAsync("Ukjent");
            }
            else
            {
                //else check if the org is in database
                organisation = await GetOrganisationByMsIdAsync(metadata);
                if(organisation is null)
                {
                    //if null, add it to the database
                    organisation = await CreateOrganisationAsync(metadata, userId);
                }
            }

            return organisation;
        }


        /// <summary>
        /// Create an organisation using the metadata. If its null, use default
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Organisation> CreateOrganisationAsync(OrganizationMetadata metadata, uint userId)
        {
            Organisation newOrg;
            if(metadata is null)
            {
                newOrg = await GetOrganisationByNameAsync("Ukjent");
            }
            else
            {
                newOrg = new Organisation() { Name = metadata.displayName, MsId = metadata.id };
            }

            await this._context.AddAsync(newOrg);
            
            await this._context.SaveChangesAsync();
            return newOrg;
        }

        /// <summary>
        /// Find the organisation by the ms id
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public async Task<Organisation> GetOrganisationByMsIdAsync(OrganizationMetadata metadata)
        {
            Organisation org = await this._context.Organisations.SingleOrDefaultAsync(o => o.MsId == metadata.id);
            return org;
        }

        /// <summary>
        /// Fidn the organisation by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Organisation> GetOrganisationByIdAsync(int id)
        {
            return await this._context.Organisations.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Find the organisation based on the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Organisation> GetOrganisationByNameAsync(string name)
        {
            return await this._context.Organisations.SingleOrDefaultAsync(x => x.Name == name);
        }
    }
}
