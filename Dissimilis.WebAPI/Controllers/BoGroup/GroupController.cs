using Dissimilis.WebAPI.Controllers.BoGroup.Commands;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoGroup.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("/{groupId:int}/addMember")]
        [ProducesResponseType(typeof(MemberAddedDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddGroupMember(int groupId, [FromBody] AddMemberDto command)
        {
            var newMemberAdded = await _mediator.Send(new AddMemberCommand(groupId, command));
            var newMember = await _mediator.Send(new QueryGroupMemberByIds(newMemberAdded.UserId, groupId));
            return Created($"User with id {newMemberAdded.UserId} add to group with id {newMember.GroupId}.", newMember);
        }

        [HttpDelete("/{groupId:int}/removeMember")]
        [ProducesResponseType(typeof(UserRemovedDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddGroupMember(int groupId, [FromBody] RemoveUserDto command)
        {
            var memberRemoved = await _mediator.Send(new RemoveUserCommand(groupId, command));
            return Ok(memberRemoved);
        }

        [HttpPatch("/{groupId:int}/changeUserRole")]
        [ProducesResponseType(typeof(UserRoleChangedDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ChangeUserRole(int groupId, [FromBody] ChangeUserRoleDto command)
        {
            var memberRoleChanged = await _mediator.Send(new ChangeUserRoleCommand(groupId, command));
            var memberUpdated = await _mediator.Send(new QueryGroupMemberByIds(memberRoleChanged.UserId, groupId));
            return Ok(memberUpdated);
        }
    }
}
