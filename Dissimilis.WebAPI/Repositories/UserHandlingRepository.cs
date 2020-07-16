﻿using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.Reposities;
using Dissimilis.WebAPI.Reposities.Interfaces;
using Experis.Ciber.Authentication.Microsoft;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Experis.Ciber.Web.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories
{
    public class UserHandlingRepository : IUserHandlingRepository
    {
        private readonly DissimilisDbContext context;
        private readonly UserRepository userRepo;
        private readonly OrganisationRepository orgRepo;
        private readonly CountryRepository countryRepo;
        public UserHandlingRepository(DissimilisDbContext context)
        {
            this.context = context;
            this.userRepo = new UserRepository(context);
            this.orgRepo = new OrganisationRepository(context);
            this.countryRepo = new CountryRepository(context);
        }
        public async Task<User> CreateOrFindUser(UserEntityMetadata userMeta, MSGraphAPI graph_api)
        {
            User user = Task.Run(() => this.userRepo.CreateOrFindUserAsync(userMeta)).Result;
            OrganizationMetadata orgData;
            try
            {
                orgData = await graph_api.GetOrganizationAsync();
            }
            catch
            {
                orgData = null;
            }
            
            if(user.OrganisationId == null)
            {
                //The create or find method handles null values
                Organisation organisation = await this.orgRepo.CreateOrFindOrganisationAsync(orgData, (uint)user.Id);
                user = await this.userRepo.UpdateUserOrganisationAsync(user, organisation);
            }

            if(user.CountryId == null)
            {
                //The create or find method handles null values
                Country country = await this.countryRepo.CreateOrFindCountryAsync(orgData, (uint)user.Id);
                user = await this.userRepo.UpdateUserCountryAsync(user, country);
            }
         
            return user;
        }
    }
}
