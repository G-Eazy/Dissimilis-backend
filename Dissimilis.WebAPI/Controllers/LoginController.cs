using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Authentication;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers
{
    public class LoginController : Experis.Ciber.Authentication.Microsoft.Controllers.LoginControllerBase
        <DissimilisServicePrincipal, DissimilisWebCredentials>
    {
        private readonly DissimilisDbContext context;

        public LoginController(DissimilisDbContext context)
        {
            this.context = context;
        }

        protected override DissimilisWebCredentials GetCredentials(UserEntityMetadata user, HttpContext httpContext, out string error)
        {
            //TODO Notes
            //User the user information to look up or create a user
            //Extract the numberic UserId from the database
 /*           User webUser = FindUser(user);*/
            error = null; //allows us to set an error. 
            return new DissimilisWebCredentials(1);
        }

        protected override DissimilisServicePrincipal GetServicePrincipal(string web_app_url)
        {
            //TODO pass web_app_url this in later into the service principal
            return new DissimilisServicePrincipal();
        }

        /// <summary>
        /// Find if the user exists and return it if it does or create otherwise
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected User FindUser (UserEntityMetadata user)
        {
            var findUser = this.context.Users.FirstOrDefault(x => x.MsId == user.id);
            if (findUser is null)
            {
                //Create user
                this.context.Users.Add(new User() {Name = user.displayName, Email = user.Email(), MsId = user.id});
            }

            context.SaveChanges();

            return findUser;
        }
    }
}
