using Dissimilis.WebAPI.Database.Models;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    public interface ICountryRepositories
    {
        Task<Country> GetCountryByIdAsync(int id);
        Task<Country> GetCountryByNameAsync(string name);
        Task<Country> CreateCountryAsync(string name);
        Task<Country> CreateOrFindCountryAsync(OrganizationMetadata metadata);
    }
}
