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
using Experis.Ciber.Web.API.Controllers;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/song")]
    [ApiController]
    public class SongController : UserControllerBase
    {
        private SongRepository repository;
        
        public SongController(DissimilisDbContext context)
        {
            this.repository = new SongRepository(context);
        }

        #region CRUD Song
        /// <summary>
        /// Create new song
        /// </summary>
        /// <param name="NewSongObject"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSong([FromBody] NewSongDTO NewSongObject)
        {
            var result = await repository.CreateSong(NewSongObject, base.UserID);
            if (result != 0)
                return base.Created($"api/song/{result}", $"{result}");
            else
                return base.BadRequest("Unable to create Song");
        }

        /// <summary>
        /// Get song by Id
        /// </summary>
        /// <returns>200</returns> 
        [HttpGet("{songId:int:min(1)}")]
        public async Task<IActionResult> GetSongById(int songId)
        {
            var SongObject = await repository.GetSongById(songId);
            if (SongObject != null)
                return base.Ok(SongObject);
            else
                return base.NotFound();
        }

        /// <summary>
        /// Search songs with optional filters
        /// </summary>
        /// <returns>200</returns>
        [HttpGet("search")]
        public async Task<ActionResult<UpdateSongDTO[]>> Search([FromQuery] SongQueryDTO SongQueryObject)
        {
            var SongDTOArray = await repository.SearchSongs(SongQueryObject);
            if (SongDTOArray.Count() != 0)
                return base.Ok(SongDTOArray);

            return base.NoContent();
        }
        
        /// <summary>
        /// Update song by Id
        /// </summary>
        /// <returns>204</returns> 
        [HttpPatch]
        public async Task<IActionResult> UpdateSong([FromBody] UpdateSongDTO UpdateSongObject)
        {
            bool result = await repository.UpdateSong(UpdateSongObject, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("Unable to update Song");
        }

        /// <summary>
        /// Delete song by Id
        /// </summary>
        /// <returns>204</returns> 
        [HttpDelete("{songId:int:min(1)}")]
        public async Task<IActionResult> DeleteSong(int songId)
        {
            bool result = await repository.DeleteSong(songId, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("Unable to delete Song");
        }
        #endregion

    }
}
