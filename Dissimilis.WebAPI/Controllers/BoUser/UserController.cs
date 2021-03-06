using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoOrganisation.Commands;
using Dissimilis.WebAPI.Controllers.BoUser.Commands;

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
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _mediator.Send(new QueryCurrentUser());
            return Ok(result);
        }

        /// <summary>
        /// Fetch the users that is in my groups and/or organizations. Fetch all or search in name or email
        /// </summary>
        /// <returns>User</returns>
        [HttpPost("myGroupUsers")]
        [ProducesResponseType(typeof(UserPagedResultDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UsersInMyGroups([FromBody] UsersInMyGroupsDto dto)
        {
            var result = await _mediator.Send(new QueryUsersInMyGroups(dto));
            return Ok(result);
        }

        /// <summary>
        /// Draft of a logout method
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Logout()
        {
            return Ok();
        }

        [HttpGet("currentUser/adminStatuses")]
        [ProducesResponseType(typeof(UserAdminStatusDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCurrentUserAdminStatuses()
        {
            var result = await _mediator.Send(new QueryUserAdminStatuses());
            return Ok(result);
        }

        [HttpGet("sysAdmins")]
        [ProducesResponseType(typeof(UserDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSysAdmins()
        {
            // TODO: Maybe add a restriction on this later
            var result = await _mediator.Send(new QuerySysAdmins());
            return Ok(result);
        }

        [HttpPatch("{userId:int}/updateSysAdminStatus")]
        [ProducesResponseType(typeof(UserDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateSysAdminStatus(int userId, [FromBody] UpdateSysAdminStatusDto command)
        {
            var item = await _mediator.Send(new UpdateSysAdminStatusCommand(userId, command));
            var result = await _mediator.Send(new QueryUserById(item.UserId));
            return Ok(result);
        }

        [HttpDelete("{userId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _mediator.Send(new DeleteUserCommand(userId));
            return Ok();
        }

    }
}
