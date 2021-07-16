using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.Query;
using Dissimilis.WebAPI.Controllers.BoVoice.Commands;
using Dissimilis.WebAPI.Controllers.BoVoice.Commands.ComponentInterval;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    [Route("api/song/{songId:int}/voice")]
    [ApiController]
    public class VoiceController : Controller
    {
        private readonly IMediator _mediator;

        public VoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateSongVoice(int songId, [FromBody] CreateSongVoiceDto command)
        {
            var item = await _mediator.Send(new CreateSongVoiceCommand(songId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, item.SongVoiceId));
            return Created($"{item.SongVoiceId}", result);
        }


        [HttpGet("{voiceId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSongVoice(int songId, int voiceId)
        {
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Ok(result);
        }


        [HttpPatch("{voiceId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateSongVoice(int songId, int voiceId, [FromBody] CreateSongVoiceDto command)
        {
            var item = await _mediator.Send(new UpdateSongVoiceCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, item.SongVoiceId));
            return Ok(result);

        }

        [HttpDelete("{voiceId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteSongVoice(int songId, int voiceId)
        {
            var item = await _mediator.Send(new DeleteSongVoiceCommand(songId, voiceId, _mediator));
            return Ok(item);
        }

        /// <summary>
        /// Duplicate voice
        /// </summary>
        [HttpPost("{voiceId:int}/duplicate")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> DuplicateVoice(int songId, int voiceId, [FromBody] CreateSongVoiceDto command)
        {
            var item = await _mediator.Send(new DuplicateVoiceCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, item.SongVoiceId));
            return Created($"{item.SongVoiceId}", result);
        }

        /// <summary>
        /// Duplicates all chords from the provided source voice with or without the component intervals included in the duplicated chords.
        /// </summary>
        [HttpPost("{voiceId:int}/duplicateAllChords")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> DuplicateAllChords(int songId, int voiceId, [FromBody] DuplicateAllChordsDto command)
        {
            var item = await _mediator.Send(new DuplicateAllChordsCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Created($"{item.SongVoiceId}", result);
        }

        /// <summary>
        /// Adds a specified component interval to all chords in the targeted voice.
        /// </summary>
        [HttpPost("{voiceId:int}/addComponentInterval")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddComponentInterval(int songId, int voiceId, [FromBody] AddComponentIntervalDto command)
        {
            var item = await _mediator.Send(new AddComponentIntervalCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Created($"{item.SongVoiceId}", result);
        }

        /// <summary>
        /// Removes a specified component interval to all chords in the targeted voice.
        /// </summary>
        [HttpPost("{voiceId:int}/removeComponentInterval")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> RemoveComponentInterval(int songId, int voiceId, [FromBody] RemoveComponentIntervalDto command)
        {
            var item = await _mediator.Send(new RemoveComponentIntervalCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Created($"{item.SongVoiceId}", result);
        }
    }
}
