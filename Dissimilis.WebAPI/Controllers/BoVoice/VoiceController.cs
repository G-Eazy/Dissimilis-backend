using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    [Route("api")]
    [ApiController]
    public class VoiceController : Controller
    {
        private readonly IMediator _mediator;

        public VoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("song/{songId:int}/part")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreatePart(int songId [FromBody] CreatePartDto command)
        {
            var item = await _mediator.Send(new CreateBarCommand(songId,  command));
            var result = await _mediator.Send(new QueryBarById(songId, partId, item.BarId));

            return Created($"{item.BarId}", result);

        }

        /// <summary>
        /// Create bar
        /// </summary>
        [HttpPost("song/{songId:int}/part/{partId:int}/bar")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateBar(int songId, int partId, [FromBody] CreateBarDto command)
        {
            var item = await _mediator.Send(new CreateBarCommand(songId, partId, command));
            var result = await _mediator.Send(new QueryBarById(songId, partId, item.BarId));

            return Created($"{item.BarId}", result);

        }

        /// <summary>
        /// Get bar
        /// </summary>
        [HttpGet("song/{songId:int}/part/{partId:int}/bar/{barId:int}")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBar(int songId, int partId, int barId)
        {
            var result = await _mediator.Send(new QueryBarById(songId, partId, barId));

            return base.Ok(result);
        }

        /// <summary>
        /// Update bar
        /// </summary>
        [HttpPatch("song/{songId:int}/part/{partId:int}/bar/{barId:int}")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBar(int songId, int partId, int barId, [FromBody] UpdateBarDto command)
        {
            var item = await _mediator.Send(new UpdateBarCommand(songId, partId, barId, command));
            var result = await _mediator.Send(new QueryBarById(songId, partId, item.BarId));
            return Ok(result);
        }

        /// <summary>
        /// Delete bar
        /// </summary>
        [HttpDelete("song/{songId:int}/part/{partId:int}/bar/{barId:int}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBar(int songId, int partId, int barId)
        {
            await _mediator.Send(new DeleteBarCommand(songId, partId, barId));
            return NoContent();


        }

    }
}
