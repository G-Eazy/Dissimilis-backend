using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Repositories;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Private variable to get the DissimilisDbContext
        private UserRepository repository;
        public UserController(DissimilisDbContext context)
        {
            this.repository = new UserRepository(context);
        }

        /// <summary>
        /// Fetch all users in the database
        /// </summary>
        /// <returns>AllUsers</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var UserDTOArray = await repository.GetAllUsersAsync();
            return Ok(UserDTOArray);
        }
    }
}
