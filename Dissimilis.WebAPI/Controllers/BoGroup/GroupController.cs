﻿using Dissimilis.WebAPI.Controllers.Bogroup.Query;
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
    [Route("api/groups")]
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
        }

        [HttpPatch("/{groupId:int}/users/{userId:int}/changeUserRole")]
        [ProducesResponseType(typeof(UserRoleChangedDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ChangeUserRole(int groupId, int userId, [FromBody] ChangeUserRoleDto command)
        {
            var memberRoleChanged = await _mediator.Send(new ChangeUserRoleCommand(groupId, userId, command));
            var memberUpdated = await _mediator.Send(new QueryGroupMemberByIds(memberRoleChanged.UserId, memberRoleChanged.GroupId));
            return Ok(memberUpdated);
        }

        [HttpPost("/{groupId:int}/users")]
        [ProducesResponseType(typeof(MemberAddedDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddGroupMember(int groupId, [FromBody] AddMemberDto command)
        { 
            var newMemberAdded = await _mediator.Send(new AddMemberCommand(groupId, command));
            var newMember = await _mediator.Send(new QueryGroupMemberByIds(newMemberAdded.UserId, groupId));
            return Created($"User with id {newMemberAdded.UserId} add to group with id {newMember.GroupId}.", newMember);
        }

        [HttpDelete("/{groupId:int}/users/{userId:int}")]
        [ProducesResponseType(typeof(MemberRemovedDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RemoveGroupMember(int groupId, int userId)
        {
            var memberRemoved = await _mediator.Send(new RemoveMemberCommand(groupId, userId));
            return Ok(memberRemoved);
        }
    }
}
