﻿using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.Commands;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.DTOs;
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

        [HttpPost("song/{songId:int}/voice")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateSongVoice(int songId, [FromBody] CreateSongVoiceDto command)
        {
            var item = await _mediator.Send(new CreateSongVoiceCommand(songId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, item.SongVoiceId));
            return Created($"{item.SongVoiceId}", result);
        }


        [HttpGet("song/{songId:int}/voice/{voiceId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSongVoice(int songId, int voiceId)
        {
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Ok(result);
        }


        [HttpPatch("song/{songId:int}/voice/{voiceId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateSongVoice(int songId, int voiceId, [FromBody] CreateSongVoiceDto command)
        {
            var item = await _mediator.Send(new UpdateSongVoiceCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, item.SongVoiceId));
            return Ok(result);

        }

        [HttpDelete("song/{songId:int}/voice/{voiceId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteSongVoice(int songId, int voiceId)
        {
            await _mediator.Send(new DeleteSongVoiceCommand(songId, voiceId));
            return NoContent();
        }

        /// <summary>
        /// Duplicate voice
        /// </summary>
        [HttpPost("song/{songId:int}/voice/{voiceId:int}/duplicate")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> DuplicateVoice(int songId, int voiceId, [FromBody] CreateSongVoiceDto command)
        {
            var item = await _mediator.Send(new DuplicateVoiceCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, item.SongVoiceId));
            return Created($"{item.SongVoiceId}", result);
        }

        /// <summary>
        /// Create bar
        /// </summary>
        [HttpPost("song/{songId:int}/voice/{voiceId:int}/bar")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateBar(int songId, int voiceId, [FromBody] CreateBarDto command)
        {
            var item = await _mediator.Send(new CreateSongBarCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongById(songId));
            return Created($"{item.SongBarId}", result);
        }


        /// <summary>
        /// Update bar
        /// </summary>
        [HttpPatch("song/{songId:int}/voice/{voiceId:int}/bar/{barId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBar(int songId, int voiceId, int barId, [FromBody] UpdateBarDto command)
        {
            var item = await _mediator.Send(new UpdateSongBarCommand(songId, voiceId, barId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Ok(result);
        }

        /// <summary>
        /// Delete bar
        /// </summary>
        [HttpDelete("song/{songId:int}/voice/{voiceId:int}/bar/{barId:int}")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBar(int songId, int voiceId, int barId)
        {
            await _mediator.Send(new DeleteSongBarCommand(songId, voiceId, barId));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Ok(result);
        }

        /// <summary>
        /// Create note
        /// </summary>
        [HttpPost("song/{songId:int}/voice/{voiceId:int}/bar/{barId:int}/note")]
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
        [HttpPatch("song/{songId:int}/voice/{voiceId:int}/bar/{barId:int}/note/{chordId:int}")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateNote(int songId, int voiceId, int barId, int chordId, [FromBody] UpdateNoteDto command)
        {
            var item = await _mediator.Send(new UpdateSongNoteCommand(songId, voiceId, barId, chordId, command));
            var result = await _mediator.Send(new QueryBarById(songId, voiceId, barId));
            return Ok(result);
        }

        /// <summary>
        /// Delete note
        /// </summary>
        [HttpDelete("song/{songId:int}/voice/{voiceId:int}/bar/{barId:int}/note/{chordId:int}")]
        [ProducesResponseType(typeof(BarDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteNote(int songId, int voiceId, int barId, int chordId)
        {
            await _mediator.Send(new DeleteSongNoteCommand(songId, voiceId, barId, chordId));
            var result = await _mediator.Send(new QueryBarById(songId, voiceId, barId));
            return Ok(result);
        }

        /// <summary>
        /// Duplicates all chords from the provided source voice with or without the component intervals included in the duplicated chords.
        /// </summary>
        [HttpPost("song/{songId:int}/voice/{voiceId:int}/duplicateAllChords")]
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
        [HttpPost("song/{songId:int}/voice/{voiceId:int}/addComponentInterval")]
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
        [HttpPost("song/{songId:int}/voice/{voiceId:int}/removeComponentInterval")]
        [ProducesResponseType(typeof(SongVoiceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> RemoveComponentInterval(int songId, int voiceId, [FromBody] RemoveComponentIntervalDto command)
        {
            var item = await _mediator.Send(new RemoveComponentIntervalCommand(songId, voiceId, command));
            var result = await _mediator.Send(new QuerySongVoiceById(songId, voiceId));
            return Created($"{item.SongVoiceId}", result);
        }
    }
}
