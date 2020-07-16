using Dissimilis.WebAPI.Database.Models;
using Experis.Ciber.Authentication.Microsoft;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Experis.Ciber.Web.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Reposities.Interfaces
{
    public interface IUserHandlingRepository
    {
        Task<User> CreateOrFindUser(UserEntityMetadata user, MSGraphAPI graph_api);
        
    }
}
