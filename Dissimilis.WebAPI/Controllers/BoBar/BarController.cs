using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dissimilis.WebAPI.Controllers.BoBar.Commands;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoBar.Query;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.Query;
using Dissimilis.WebAPI.Controllers.BoSong.Query;

namespace Dissimilis.WebAPI.Controllers.BoBar
{
    [Route("api/song/{songId:int}/voice/{voiceId:int}/bar")]
    [ApiController]
    public class BarController : Controller
    {
        private readonly IMediator _mediator;

        public BarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create bar
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateBar(int songId, int voiceId, [FromBody] CreateBarDto command)
        {
            var item = await _mediator.Send(new CreateSongBarCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongById(songId));
            return Created($"{item.SongBarId}", result);
        }


        /// <summary>
        /// Update bar
        /// </summary>
        [HttpPatch("{barId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBar(int songId, int voiceId, int barId, [FromBody] UpdateBarDto command)
        {
            await _mediator.Send(new UpdateSongBarCommand(songId, voiceId, barId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Ok(result);
        }

        /// <summary>
        /// Delete bar
        /// </summary>
        [HttpDelete("{barId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBar(int songId, int voiceId, int barId)
        {
            await _mediator.Send(new DeleteSongBarCommand(songId, voiceId, barId));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Ok(result);
        }
    }
}
