using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoOrganisation.Query;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation
{
        [Route("api/organisations")]
        [ApiController]
        public class OrganisationController : Controller
        {
            private readonly IMediator _mediator;

            public OrganisationController(IMediator mediator)
            {
                _mediator = mediator;
            }

            /// <summary>
            /// get all organisations filtered
            /// </summary>
            [HttpGet("")]
            [ProducesResponseType(typeof(OrganisationIndexDto[]), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.NotFound)]
            public async Task<IActionResult> GetOrganisations([FromQuery] string filterBy)
            {
                var result = await _mediator.Send(new QueryGetOrganisations(filterBy));
                return Ok(result);
            }
        }
}
