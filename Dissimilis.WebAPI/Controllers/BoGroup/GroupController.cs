using Dissimilis.WebAPI.Controllers.Bogroup.Query;
using Dissimilis.WebAPI.Controllers.BoGroup.Commands;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoGroup.Queries;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.Bousers.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    [Route("api/group")]
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
        [HttpPost]
        [ProducesResponseType(typeof(GroupByIdDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto command)
        [HttpPost("/{groupId:int}/addMember")]
        [ProducesResponseType(typeof(MemberAddedDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddGroupMember(int groupId, [FromBody] AddMemberDto command)
        {
            var item = await _mediator.Send(new CreateGroupCommand(command));
            var result = await _mediator.Send(new QueryGroupById(item.GroupId));

            return Created($"{result.GroupId}", result);
        }

        [HttpGet("{groupId:int}")]
        [ProducesResponseType(typeof(GroupByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            var group = await _mediator.Send(new QueryGroupById(groupId));
            return Ok(group);
            var newMemberAdded = await _mediator.Send(new AddMemberCommand(groupId, command));
            var newMember = await _mediator.Send(new QueryGroupMemberByIds(newMemberAdded.UserId, groupId));
            return Created($"User with id {newMemberAdded.UserId} add to group with id {newMember.GroupId}.", newMember);
        }

        [HttpGet("{groupId:int}/users")]
        [ProducesResponseType(typeof(UserDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUsersInGroup(int groupId)
        [HttpDelete("/{groupId:int}/removeMember/{userId:int}")]
        [ProducesResponseType(typeof(MemberRemovedDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddGroupMember(int groupId, int userId)
        {
            var users = await _mediator.Send(new QueryUsersInGroup(groupId));
            return Ok(users);
            var memberRemoved = await _mediator.Send(new RemoveMemberCommand(groupId, userId));
            return Ok(memberRemoved);
        }
    }
}
