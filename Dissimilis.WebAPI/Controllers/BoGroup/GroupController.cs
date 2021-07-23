using Dissimilis.WebAPI.Controllers.Bogroup.Query;
using Dissimilis.WebAPI.Controllers.BoGroup.Commands;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
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

        [HttpGet("{groupId:int}/users")]
        [ProducesResponseType(typeof(UserDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUsersInGroup(int groupId)
        {
            var users = await _mediator.Send(new QueryUsersInGroup(groupId));
            return Ok(users);
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
    }
}
