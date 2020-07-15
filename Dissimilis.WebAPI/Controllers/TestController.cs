using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Experis.Ciber.Web.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Experis.Ciber.Web.API.Controllers;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : Experis.Ciber.Web.API.Controllers.UserControllerBase
    {
        //Private variable to get the DissimilisDbContext
        private DissimilisDbContext context;
        public TestController(DissimilisDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Test method to get userId
        /// </summary>
        /// <returns></returns>
        [HttpGet("userid")]
        public ActionResult<string> MyTestMethod()
        {
            return base.Ok("My user ID is " + this.UserID);
        }

        /// <summary>
        /// A test fucntion to fetch all users that are in the database
        /// </summary>
        /// <returns>AllUsers</returns>
        [HttpGet("users")]
        public ActionResult<Database.Models.User[]> GetAllUsers()
        {
            //This return will return the users + their country and usergroup
            var userId = this.UserID;

            var allUsers = this.context.Users
                .Include(x => x.Country)
                .Include(x => x.Organisation)
                .ToArray();

            if (allUsers.Count() == 0) return base.NoContent();
            return allUsers;
        }




        /// <summary>
        /// This is a test method to fetch the api using a name and a number
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bob"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public ActionResult<string> MyMethod(string name, int bob)
        {
            //200.ok this just sends a 200.ok to the browser and inserts a text
            return base.Ok("hello, this is " + name + " your integer is " + bob);
        }
    }
}
