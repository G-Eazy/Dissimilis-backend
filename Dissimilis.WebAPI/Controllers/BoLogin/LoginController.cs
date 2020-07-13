using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Authentication;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers
{
    public class LoginController : Experis.Ciber.Authentication.Microsoft.Controllers.LoginControllerBase
        <DissimilisServicePrincipal, DissimilisWebCredentials>
    {
        private readonly DissimilisDbContext _context;
        //Private variable to get the DissimilisDbContext
        
        
        public LoginController(DissimilisDbContext context)
        {
            this._context = context;
        }

        protected override DissimilisWebCredentials GetCredentials(UserEntityMetadata user, HttpContext httpContext, out string error)
        {
            //TODO Notes
            //User the user information to look up or create a user
            //Extract the numberic UserId from the database
            User webUser = FindLoggedinUser(user);
            error = null; //allows us to set an error. 
            return new DissimilisWebCredentials(Convert.ToUInt32(webUser.Id)); //Convert.ToUInt32(webUser.Id)
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
        protected User FindLoggedinUser(UserEntityMetadata user)
        {

            var findUser = _context.Users.SingleOrDefault(x => x.MsId == user.id);
            if (findUser is null)
            {
                findUser = _context.Users.SingleOrDefault(x => x.Email == user.Email());
                if (findUser is null)
                {
                    //Create user
                    findUser = new User() { Name = user.displayName, Email = user.Email(), MsId = user.id, OrganisationId = 1, CountryId = 1 };
                    this._context.Users.Add(findUser);
                    _context.SaveChanges();

                    this._context.UserGroupMembers.Add(new UserGroupMembers() { UserGroupId = 2, UserId = findUser.Id });
                }
                else
                {
                    // Update user that has same name with msID
                    findUser.MsId = user.id;
                }
            }

            _context.SaveChanges();

            return findUser;
        }

    }
}
