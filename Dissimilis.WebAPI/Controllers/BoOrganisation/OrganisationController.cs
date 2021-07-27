﻿using Dissimilis.WebAPI.Controllers.BoOrganisation.Commands;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoOrganisation.Query;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.Bousers.Query;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;
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

        [HttpPost]
        [ProducesResponseType(typeof(OrganisationByIdDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationDto command)
        {
            var item = await _mediator.Send(new CreateOrganisationCommand(command));
            var result = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));
            return Created($"{result.Id}", result);
        }

        [HttpGet("{organisationId:int}")]
        [ProducesResponseType(typeof(OrganisationByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetOrganisationById(int organisationId)
        {
            var organisation = await _mediator.Send(new QueryOrganisationById(organisationId));
            return Ok(organisation);
        }

        [HttpGet("{organisationId:int}/users")]
        [ProducesResponseType(typeof(UserDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUsersInOrganisation(int organisationId)
        {
            var users = await _mediator.Send(new QueryUsersInOrganisation(organisationId));
            return Ok(users);
        }

        /// <summary>
        /// get all organisations filtered
        /// </summary>
        [HttpGet("")]
        [ProducesResponseType(typeof(OrganisationIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetOrganisations([FromQuery] string filterByRole)
        {
            var result = await _mediator.Send(new QueryGetOrganisations(filterByRole));
            return Ok(result);
        }

        [HttpPatch("{organisationId:int}")]
        [ProducesResponseType(typeof(OrganisationByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateOrganisation(int organisationId, [FromBody] UpdateGroupAndOrganisationDto command)
        {
            var item = await _mediator.Send(new UpdateOrganisationCommand(organisationId, command));
            var organisation = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));
            return Ok(organisation);
        }

        [HttpPost("{organisationId:int}/users")]
        [ProducesResponseType(typeof(UserOrganisationAddedDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddOrganisationUser(int organisationId, [FromBody] AddUserOrganisationDto command)
        {
            var organisationUser = await _mediator.Send(new AddUserOrganisationCommand(organisationId, command));
            return Ok(organisationUser);
        }
    }
}
