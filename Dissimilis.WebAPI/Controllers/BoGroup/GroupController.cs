using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    [Route("api/Group")]
    [ApiController]
    public class GroupController : Controller
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// getGroups
        /// </summary>
        [HttpGet("getGroups")]
        [ProducesResponseType(typeof(GroupIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetGroups([FromBody] GetGroupsQueryDto command)
        {
            var result = await _mediator.Send(new QueryGetGroups(command));
            return Ok(result);
        }
    }
}
