using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Threading;
using Dissimilis.WebAPI.Repositories;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/songs")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private SongRepository _repository;
        
        public SongController(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
        }

        /// <summary>
        /// Get song by Id
        /// </summary>
        /// <returns>200</returns> 
        [HttpGet("{Id:int:min(1)}")]
        public async Task<IActionResult> GetSongById(int Id)
        {
            var SuperObject = new SuperDTO(Id);
            var SongObject = await _repository.GetSongByIdQuery(SuperObject);
            if (SongObject != null)
                return base.Ok(SongObject);
            else
                return base.BadRequest("No song by that Id");
        }

        /// <summary>
        /// Fetch songs that contain {Title} and/or from Arranger {ArrangerId} in the database. Limit output to {Num} songs. Set {OrderByDateTime} to true for ordering.
        /// </summary>
        /// <returns>200</returns>
        [HttpGet("search")] 
        public async Task<ActionResult<SongDTO[]>> Search([FromQuery] SongSearchDTO SongSearchObject)
        {
            var SongDTOArray = await _repository.SearchQuery(SongSearchObject);
            if (SongDTOArray.Length == 0)
                return base.BadRequest("No arranger by that Id");
            else 
                return base.Ok(SongDTOArray);
        }
        



        /// <summary>
        /// Create new song. Arranger must be id of somebody in DB, see all users below.
        /// </summary>
        /// <param name="NewSongDTO"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSong([FromBody] NewSongDTO NewSongObject)
        {
            var result = await _repository.CreateSongCommand(NewSongObject);
            if (result != null)
                return base.Created($"api/songs/{result.Id}", ""); // Add result.Id as second param if frontend wants it in body
            else
                return base.BadRequest("No arranger by that Id");

        }
        
        
        /// <summary>
        /// Update song by Id
        /// </summary>
        /// <returns>200</returns> 
        [HttpPatch("{Id:int:min(1)}")]
        public async Task<IActionResult> UpdateSong(int Id)
        {
            var UpdateSongObject = new UpdateSongDTO(Id);
            bool result = await _repository.UpdateSongCommand(UpdateSongObject);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("No song by that Id");
        }

        /// <summary>
        /// Delete song by Id
        /// </summary>
        /// <returns>200</returns> 
        [HttpDelete("{Id:int:min(1)}")]
        public async Task<IActionResult> DeleteSong(int Id)
        {
            var DeleteSongObject = new SuperDTO(Id);
            bool result = await _repository.DeleteSongCommand(DeleteSongObject);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("No song by that Id");
        }        
    }
}
