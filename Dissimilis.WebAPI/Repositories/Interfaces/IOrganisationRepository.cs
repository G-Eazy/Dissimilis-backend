using Dissimilis.WebAPI.Database.Models;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Reposities.Interfaces
{
    public interface IOrganisationRepository
    {
        Task<Organisation> GetOrganisationByIdAsync(int id);
        Task<Organisation> GetOrganisationByNameAsync(string name);
        Task<Organisation> CreateOrganisationAsync(OrganizationMetadata metadata);
        Task<Organisation> CreateOrFindOrganisationAsync(OrganizationMetadata metadata);
        Task<Organisation> FindOrganisationByMsIdAsync(OrganizationMetadata metadata);

    }
}
