using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation
{
    [Route("api/organisation")]
    [ApiController]
    public class OrganisationController : Controller
    {
        private readonly IMediator _mediator;

        public OrganisationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(GroupByIdDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationDto command)
        {
            var item = await _mediator.Send(new CreateOrganisationCommand(command));
            var result = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));
            return Created($"{result.OrganisationId}", result);
        }
    }
}
