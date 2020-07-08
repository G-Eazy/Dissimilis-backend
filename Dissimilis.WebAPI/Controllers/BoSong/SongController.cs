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
using Dissimilis.WebAPI.Controllers.BoSong.Queries;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        //Private variable to get the DissimilisDbContext
        private DissimilisDbContext context;
        private IMediator _mediator;
        public SongController(IMediator _mediator)
        {
            this._mediator = _mediator;
        }

        /// <summary>
        /// Fetch all songs in the database
        /// </summary>
        /// <returns>200</returns>
        [HttpGet("")]
        public async Task<IActionResult> GetAllSongs()
        {
            var SongDTOArray = await _mediator.Send(new QueryAllSongs());
            return Ok(SongDTOArray);
        }
    }
}
