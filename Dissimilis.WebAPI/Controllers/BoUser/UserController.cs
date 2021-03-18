using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser
{
    // TODO add authorization
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Fetch all users in the database
        /// </summary>
        /// <returns>AllUsers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new QueryAll());
            return Ok(result);
        }

        /// <summary>
        /// Fetch the current user from the database
        /// </summary>
        /// <returns>User</returns>
        [HttpGet("currentUser")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUser()
        {
            var result = await _mediator.Send(new QueryUserById());
            return Ok(result);
        }
    }
}
