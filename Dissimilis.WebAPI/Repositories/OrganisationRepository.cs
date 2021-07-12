using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Repositories
{
    public class OrganisationRepository 
    {
        private DissimilisDbContext _context;
        public OrganisationRepository(DissimilisDbContext context)
        {
            this._context = context;
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
