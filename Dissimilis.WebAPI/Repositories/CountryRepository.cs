using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Microsoft.EntityFrameworkCore;
using Experis.Ciber.Authentication.Microsoft.APIObjects;

namespace Dissimilis.WebAPI.Repositories
{
    public class CountryRepository
    {
        private readonly DissimilisDbContext context;

        public CountryRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Create a new country entry based on the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> CreateCountryAsync(string name)
        {
            //Double check in case the name already exists
            Country country = await GetCountryByNameAsync(name);
            if (country is null)
            {
                country = new Country(name);
                await this.context.Countries.AddAsync(country);

                await this.context.SaveChangesAsync();
            }

            return country;
        }

        /// <summary>
        /// Create or find country based on metadata
        /// if metadata is null, use default country "Norway"
        /// </summary>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public async Task<Country> CreateOrFindCountryAsync(OrganizationMetadata metaData)
        {
            Country country;
            if (metaData is null)
            {
                country = await GetCountryByNameAsync("Norge");
            }
            else
            {
                //Look for the user country, if it doesn't exists, create a new entry in db
                country = await GetCountryByNameAsync(metaData.country.ToString());
                if (country is null)
                {
                    country = await CreateCountryAsync(metaData.country.ToString());
                }
            }

            return country;
        }



        /// <summary>
        /// Get country based on country Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> GetCountryByNameAsync(string name)
        {
            return await context.Countries.SingleOrDefaultAsync(c => c.Name == name);
        }
    }
}
