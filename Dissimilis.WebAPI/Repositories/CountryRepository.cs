using Dissimilis.WebAPI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Repositories.Interfaces;
using Dissimilis.WebAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using Experis.Ciber.Authentication.Microsoft.APIObjects;

namespace Dissimilis.WebAPI.Repositories
{
    public class CountryRepository : ICountryRepositories
    {
        private readonly DissimilisDbContext _context;
        public CountryRepository(DissimilisDbContext context)
        {
            this._context = context;
        }

        public async Task<Country> CreateCountryAsync(string name)
        {
            var country = new Country(name);
            await this._context.Countries.AddAsync(country);
            await this._context.SaveChangesAsync();
            return country;
        }

        public async Task<Country> CreateOrFindCountryAsync(OrganizationMetadata metaData)
        {
            String countryName;
            if(metaData is null)
            {
                countryName = "Norway";
            }
            else
            {
                countryName = metaData.country.ToString();
            }

            Country country = await GetCountryByNameAsync(countryName);
            
            if (country is null)
            {
                country = await CreateCountryAsync(countryName);
            }

            return country;
        }


        public async Task<Country> GetCountryByIdAsync(int id)
        {
            return await this._context.Countries.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Country> GetCountryByNameAsync(string name)
        {
            return await this._context.Countries.SingleOrDefaultAsync(c => c.Name == name);
        }
    }
}
