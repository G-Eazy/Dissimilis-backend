using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Repositories;
using Experis.Ciber.Web.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers.BoLogin
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginCheckController : UserControllerBase
    {
        private UserRepository repository;
        public LoginCheckController(UserRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("")]
        public IActionResult checkApikey()
        {
            this.repository.GetUserByIdAsync((int)base.UserID);

            return base.Ok("Your ID is still valid");
        }
    }
}
