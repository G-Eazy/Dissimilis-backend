using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.Reposities.Interfaces;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Reposities
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private DissimilisDbContext _context;
        public OrganisationRepository(DissimilisDbContext context)
        {
            this._context = context;
        }

        public async Task<Organisation> CreateOrFindOrganisationAsync(OrganizationMetadata metadata)
        {
            String orgName;
            if (metadata is null)
            {
                orgName = "Dissimilis Norge";
            }
            else
            {
                orgName = metadata.displayName;
            }
            var Organisation = GetOrganisationByNameAsync(orgName);
            if(Organisation is null)
            {
                Organisation = CreateOrganisationAsync(metadata);
            }
            return await Organisation;
        }


        public async Task<Organisation> CreateOrganisationAsync(OrganizationMetadata metadata)
        {
            Organisation newOrg;
            if(metadata is null)
            {
                var OrgName = "Dissimilis Norge";
                newOrg = new Organisation() { Name = OrgName };
            }
            else
            {
                newOrg = new Organisation() { Name = metadata.displayName, MsId = metadata.id };
            }

            await this._context.AddAsync(newOrg);
            await this._context.SaveChangesAsync();
            return newOrg;
        }

        public async Task<Organisation> FindOrganisationByMsIdAsync(OrganizationMetadata metadata)
        {
            Organisation org = await this._context.Organisations.SingleOrDefaultAsync(o => o.MsId == metadata.id);
            return org;
        }

        public async Task<Organisation> GetOrganisationByIdAsync(int id)
        {
            return await this._context.Organisations.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Organisation> GetOrganisationByNameAsync(string name)
        {
            return await this._context.Organisations.SingleOrDefaultAsync(x => x.Name == name);
        }
    }
}
