using System;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Authentication;
using Dissimilis.WebAPI.Repositories;
using Experis.Ciber.Authentication.Microsoft;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.AspNetCore.Http;

namespace Dissimilis.WebAPI.Controllers
{
    public class AuthenticationController : Experis.Ciber.Authentication.Microsoft.Controllers.LoginControllerBase
        <DissimilisServicePrincipal, DissimilisWebCredentials>
    {
        private readonly UserHandlingRepository _repository;
        //Private variable to get the DissimilisDbContext
        
        
        public AuthenticationController(DissimilisDbContext context)
        {
            this._repository = new UserHandlingRepository(context);
        }

        protected override DissimilisWebCredentials GetCredentials(UserEntityMetadata user, MSGraphAPI graphApi, HttpContext httpContext, out string error)
        {
            //Find the webuser in the db
            User webUser = Task.Run(() => this._repository.CreateOrFindUser(user, graphApi)).Result;
            //handle error if webuser is null
            if (webUser is null)
            {
                error = "There was an error loggin you in, your credentials are not valid";
                return null;
            }
            else
            {
                error = null;
                return new DissimilisWebCredentials(Convert.ToUInt32(webUser.Id));
            }
        }


        protected override DissimilisServicePrincipal GetServicePrincipal(string webAppUrl)
        {
            return new DissimilisServicePrincipal(webAppUrl);
        }
    }
}
