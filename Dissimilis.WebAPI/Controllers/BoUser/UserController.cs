using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Private variable to get the DissimilisDbContext
        private IMediator _mediator;
        public UserController(IMediator _mediator)
        {
            this._mediator = _mediator;
        }

        /// <summary>
        /// Fetch all users in the database
        /// </summary>
        /// <returns>AllUsers</returns>
        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            var UserDTOArray = await _mediator.Send(new QueryAllUsers());
            return Ok(UserDTOArray);
        }
    }
}
