using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    // TODO add authorization
    [Route("api/song")]
    [ApiController]
    public class SongController : Controller
    {
        private readonly IMediator _mediator;

        public SongController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Search songs with optional filters
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(SongIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<UpdateSongDto[]>> Search([FromQuery] SearchQueryDto command)
        {
            var result = _mediator.Send(new QuerySongSearch(command));
            return Ok(result);
        }


        /// <summary>
        /// Create song
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateSong([FromBody] CreateSongDto command)
        {
            var item = await _mediator.Send(new CreateSongCommand(command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Created($"{result.SongId}", result);
        }

        /// <summary>
        /// Get song by Id
        /// </summary>
        [HttpGet("{songId:int}")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetSongById(int songId)
        {
            var result = await _mediator.Send(new QuerySongById(songId));
            return Ok(result);
        }


        /// <summary>
        /// Update song by id
        /// </summary>
        [HttpPatch("{songId:int}")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateSong(int songId, [FromBody] UpdateSongDto command)
        {
            var item = await _mediator.Send(new UpdateSongCommand(songId, command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Ok(result);
        }


        /// <summary>
        /// Delete song by id
        /// </summary>
        [HttpDelete("{songId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteSong(int songId)
        {
            var item = await _mediator.Send(new DeleteSongCommand(songId));
            return NoContent();
        }


    }
}
