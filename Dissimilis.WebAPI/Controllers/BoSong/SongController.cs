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
    [Route("api/songs")]
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
        /// Get song by Id
        /// </summary>
        /// <returns>200</returns> 
        [HttpGet("{Id:int:min(1)}")]
        public async Task<IActionResult> GetSongById(int Id)
        {
            var SuperObject = new SuperDTO(Id);
            var SongObject = await repository.GetSongById(SuperObject);
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
        public async Task<ActionResult<SongDTO[]>> Search([FromQuery] SongQueryDTO SongQueryObject)
        {
            var SongDTOArray = await repository.SearchSongs(SongSearchObject);
            if (SongDTOArray.Length == 0)
                return base.BadRequest("No arranger by that Id");
            else
                return base.Ok(SongDTOArray);
        }


        /// <summary>
        /// Create new song. Arranger must be id of somebody in DB, see all users below.
        /// </summary>
        /// <param name="NewSongObject"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSong([FromBody] NewSongDTO NewSongObject)
        {
            var result = await repository.CreateSong(NewSongObject, base.UserID);
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
        public async Task<IActionResult> UpdateSong(int Id, [FromBody] UpdateSongDTO UpdateSongObject)
        {
            if (Id != UpdateSongObject.Id)
                return base.BadRequest("Url Id must match SongId");

            var UpdateSongObject = new UpdateSongDTO(Id);
            bool result = await repository.UpdateSong(UpdateSongObject, base.UserID);
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
            bool result = await repository.DeleteSong(DeleteSongObject, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("No song by that Id");
        }
        #endregion

    }
}
