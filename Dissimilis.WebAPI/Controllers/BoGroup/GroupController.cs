using Dissimilis.WebAPI.Controllers.BoGroup.Commands;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoGroup.Query;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    [Route("api/organisations")]
    [ApiController]
    public class GroupController : Controller
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create group
        /// </summary>
        [HttpPost("groups")]
        [ProducesResponseType(typeof(GroupByIdDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto command)
        {
            var item = await _mediator.Send(new CreateGroupCommand(command));
            var result = await _mediator.Send(new QueryGroupById(item.GroupId));

            return Created($"{result.GroupId}", result);
        }

        [HttpGet("groups/{groupId:int}")]
        [ProducesResponseType(typeof(GroupByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            var group = await _mediator.Send(new QueryGroupById(groupId));
            return Ok(group);
        }

        [HttpPatch("groups/{groupId:int}/users/{userId:int}/changeUserRole")]
        [ProducesResponseType(typeof(UserRoleChangedDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ChangeUserRole(int groupId, int userId, [FromBody] ChangeUserRoleDto command)
        {
            var memberRoleChanged = await _mediator.Send(new ChangeUserRoleCommand(groupId, userId, command));
            var memberUpdated = await _mediator.Send(new QueryGroupMemberByIds(memberRoleChanged.UserId, memberRoleChanged.GroupId));
            return Ok(memberUpdated);
        }

        /// <summary>
        /// get all groups in a given organisation filtered
        /// </summary>
        [HttpGet("{organisationId:int}/groups")]
        [ProducesResponseType(typeof(GroupIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetGroupsInOrganisation(int organisationId, [FromQuery] string filterBy)
        {
            var result = await _mediator.Send(new QueryGetGroups(filterBy, organisationId));
            return Ok(result);
        }

        /// <summary>
        /// get all groups filtered
        /// </summary>
        [HttpGet("groups")]
        [ProducesResponseType(typeof(GroupIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetGroups([FromQuery] string filterBy)
        {
            var result = await _mediator.Send(new QueryGetGroups(filterBy, null));
            return Ok(result);
        }

        [HttpPost("groups/{groupId:int}/users")]
        [ProducesResponseType(typeof(MemberAddedDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddGroupMember(int groupId, [FromBody] AddMemberDto command)
        { 
            var newMemberAdded = await _mediator.Send(new AddMemberCommand(groupId, command));
            var newMember = await _mediator.Send(new QueryGroupMemberByIds(newMemberAdded.UserId, groupId));
            return Created($"User with id {newMemberAdded.UserId} add to group with id {newMember.GroupId}.", newMember);
        }

        [HttpDelete("groups/{groupId:int}/users/{userId:int}")]
        [ProducesResponseType(typeof(MemberRemovedDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveGroupMember(int groupId, int userId)
        {
            var memberRemoved = await _mediator.Send(new RemoveMemberCommand(groupId, userId));
            return Ok(memberRemoved);
        }

        [HttpPatch("groups/{groupId:int}")]
        [ProducesResponseType(typeof(GroupByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] UpdateGroupAndOrganisationDto command)
        {
            var item = await _mediator.Send(new UpdateGroupCommand(groupId, command));
            var group = await _mediator.Send(new QueryGroupById(item.GroupId));
            return Ok(group);
        }

        [HttpDelete("groups/{groupId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var res = await _mediator.Send(new DeleteGroupCommand(groupId));
            return Ok(res);
        }

        [HttpGet("groups/{groupId:int}/users")]
        [ProducesResponseType(typeof(UserDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUsersInGroup(int groupId)
        {
            var users = await _mediator.Send(new QueryUsersInGroup(groupId));
            return Ok(users);
        }
    }
}
