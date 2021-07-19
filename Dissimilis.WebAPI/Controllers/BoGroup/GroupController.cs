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
        {
            var item = await _mediator.Send(new CreateGroupCommand(command));
            var result = await _mediator.Send(new QueryGroupById(item.SongId));

            return Created($"{result.GroupId}", result);
        }
    }
}
