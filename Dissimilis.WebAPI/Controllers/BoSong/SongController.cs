using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
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
        /// Songs created by or arranged by user
        /// </summary>
        [HttpGet("mylibrary")]
        [ProducesResponseType(typeof(SongIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> MyLibrary()
        {
            var result = await _mediator.Send(new QuerySongToLibrary());
            return Ok(result);
        }

        /// <summary>
        /// Search songs with optional filters
        /// </summary>
        [HttpPost("search")]
        [ProducesResponseType(typeof(SongIndexDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Search([FromBody] SearchQueryDto command)
        {
            var result = await _mediator.Send(new QuerySongSearch(command));
            return Ok(result);
        }


        /// <summary>
        /// Create song
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateSong([FromBody] CreateSongDto command)
        {
            var item = await _mediator.Send(new CreateSongCommand(command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Created($"{result.SongId}", result);
        }

        /// <summary>
        /// Create transposed copy of song
        /// </summary>
        [HttpPost("{songId:int}/transpose")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateTransposedSong(int songId, [FromBody] TransposeSongDto command)
        {
            var item = await _mediator.Send(new CreateTransposedSongCommand(songId, command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Ok(result);
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
        /// Get metadata for a song by Id
        /// </summary>
        [HttpGet("{songId:int}/metadata")]
        [ProducesResponseType(typeof(SongMetadataDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMetadataSongById(int songId)
        {
            var result = await _mediator.Send(new QueryMetadataSongById(songId));
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

        /// <summary>
        /// Copy bars from one position to another across all voices
        /// </summary>
        [HttpPost("{songId:int}/copyBars")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CopyBars(int songId, [FromBody] CopyBarDto command)
        {
            var item = await _mediator.Send(new CopyBarsCommand(songId, command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Ok(result);
        }

        /// <summary>
        /// Move bars from one position to another across all voices
        /// </summary>
        [HttpPost("{songId:int}/moveBars")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> MoveBars(int songId, [FromBody] MoveBarDto command)
        {
            var item = await _mediator.Send(new MoveBarsCommand(songId, command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Ok(result);
        }

        /// <summary>
        /// Delete a number of bars from a position  
        /// </summary>
        [HttpPost("{songId:int}/deleteBars")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBars(int songId, [FromBody] DeleteBarDto command)
        {
            var item = await _mediator.Send(new DeleteBarsCommand(songId, command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Ok(result);
        }

        /// <summary>
        /// Duplicate song
        /// </summary>
        [HttpPost("{songId:int}/duplicateSong")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DuplicateSong(int songId, [FromBody] DuplicateSongDto command)
        {
            var item = await _mediator.Send(new DuplicateSongCommand(songId, command));
            var result = await _mediator.Send(new QuerySongById(item.SongId));

            return Ok(result);
        }

        [HttpPatch("{songId:int}/restore")]
        [ProducesResponseType(typeof(SongByIdDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RestoreSong(int songId)
        {
            var item = await _mediator.Send(new RestoreDeletedSongCommand(songId));
            var result = await _mediator.Send(new QuerySongById(item.SongId));
            return Ok(result);
        }

    }
}
