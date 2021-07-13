using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoBar.Query;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoNote
{
    [Route("api/song/{songId:int}/voice/{voiceId:int}/bar/{barId:int}/note")]
    [ApiController]
    public class NoteController : Controller
    {
        private readonly IMediator _mediator;

        public NoteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create note
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateNote(int songId, int voiceId, int barId, [FromBody] CreateNoteDto command)
        {
            var item = await _mediator.Send(new CreateSongNoteCommand(songId, voiceId, barId, command));
            var result = await _mediator.Send(new QueryBarById(songId, voiceId, barId));
            return Created($"{item.SongChordId}", result);
        }

        /// <summary>
        /// Update note
        /// </summary>
        [HttpPatch("{chordId:int}/")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateNote(int songId, int voiceId, int barId, int chordId, [FromBody] UpdateNoteDto command)
        {
            await _mediator.Send(new UpdateSongNoteCommand(chordId, command));
            var result = await _mediator.Send(new QueryBarById(songId, voiceId, barId));
            return Ok(result);
        }

        /// <summary>
        /// Delete note
        /// </summary>
        [HttpDelete("{chordId:int}/")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteNote(int songId, int voiceId, int barId, int chordId)
        {
            await _mediator.Send(new DeleteSongNoteCommand(songId, voiceId, barId, chordId));
            var result = await _mediator.Send(new QueryBarById(songId, voiceId, barId));
            return Ok(result);
        }
    }

    [Route("api/note")]
    [ApiController]
    public class GlobalNoteController : Controller
    {
        /// <summary>
        /// Used for fetching all possible chord names.
        /// </summary>
        [HttpGet("chordOptions/")]
        [ProducesResponseType(typeof(ChordOptionsDto), (int)HttpStatusCode.OK)]
        public IActionResult GetChordOptions()
        {
            var result = GetChordOptionsCommandHandler.Handle();
            return Ok(result);
        }

        /// <summary>
        /// Used for fetching all possible single notes.
        /// </summary>
        [HttpGet("singleNoteOptions/")]
        [ProducesResponseType(typeof(SingleNoteOptionsDto), (int)HttpStatusCode.OK)]
        public IActionResult GetSingleNoteOptions()
        {
            var result = GetSingleNoteOptionsCommandHandler.Handle();
            return Ok(result);
        }
    }
}
