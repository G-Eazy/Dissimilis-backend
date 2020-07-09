using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.Controllers.SuperDTOs;
using Dissimilis.WebAPI.Controllers.BoSong.Queries;
using Dissimilis.WebAPI.Controllers.BoSong.DTOs;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        //Private variable to get the DissimilisDbContext
        private IMediator _mediator;
        public SongController(IMediator _mediator)
        {
            this._mediator = _mediator;
        }

        /// <summary>
        /// Fetch all songs in the database
        /// </summary>
        /// <returns>200</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var SongDTOArray = await _mediator.Send(new QueryAllSongs());
            return Ok(SongDTOArray);
        }

        /// <summary>
        /// Create new song. Arranger must be id of somebody in DB, see below.
        /// </summary>
        /// <param name="NewSongDTO"></param>
        /// <returns>201</returns>

        [HttpPost]
        public async Task<IActionResult> CreateSong([FromBody] NewSongDTO NewSongObject)
        {
            var SongDTO = await _mediator.Send(new CreateSongCommand(NewSongObject));
            return Created("", SongDTO);

        }

        /// <summary>
        /// Delete song by Id
        /// </summary>
        /// <returns>200</returns> 
        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> DeleteSong(int Id)
        {
            var DeleteSongObject = new SuperDTO(Id);
            var result = await _mediator.Send(new DeleteSongCommand(DeleteSongObject));
            if (result != null)
                return Ok("Removed song: " + result.Id);
            else
                return NoContent();
        }

    }
}
