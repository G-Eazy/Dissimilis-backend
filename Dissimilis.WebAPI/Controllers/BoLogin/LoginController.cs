using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Authentication;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.Repositories;
using Experis.Ciber.Authentication.Microsoft;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers
{
    public class LoginController : Experis.Ciber.Authentication.Microsoft.Controllers.LoginControllerBase
        <DissimilisServicePrincipal, DissimilisWebCredentials>
    {
        private readonly UserHandlingRepository _repository;
        //Private variable to get the DissimilisDbContext
        
        
        public LoginController(DissimilisDbContext context)
        {
            this._repository = new UserHandlingRepository(context);
        }

        protected override DissimilisWebCredentials GetCredentials(UserEntityMetadata user, MSGraphAPI graph_api, HttpContext httpContext, out string error)
        {
            //Find the webuser in the db
            User webUser = this._repository.CreateOrFindUser(user, graph_api);

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


        protected override DissimilisServicePrincipal GetServicePrincipal(string web_app_url)
        {
            return new DissimilisServicePrincipal(web_app_url);
        }
    }
}
