using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        /// get all groups in a given organisation filtered
        /// </summary>
        [HttpGet("/organisation/{organisationId:int}/groups?filter={filterBy:string}")]
        [ProducesResponseType(typeof(GroupIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetGroupsInOrganisation(int organisationId, string filterBy)
        {
            var result = await _mediator.Send(new QueryGetGroups(filterBy, organisationId));
            return Ok(result);
        }

        /// <summary>
        /// get all groups filtered
        /// </summary>
        [HttpGet("/groups?filter={filterBy:string}")]
        [ProducesResponseType(typeof(GroupIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetGroups(string filterBy)
        {
            var result = await _mediator.Send(new QueryGetGroups(filterBy, null));
            return Ok(result);
        }
    }
}
