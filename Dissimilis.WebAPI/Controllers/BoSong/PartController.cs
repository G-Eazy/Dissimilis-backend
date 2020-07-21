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
    public class PartController : UserControllerBase
    {
        private PartRepository repository;
        
        public PartController(DissimilisDbContext context)
        {
            this.repository = new PartRepository(context);
        }

        #region CRUD Part

        /// <summary>
        /// Get part by Id
        /// </summary>
        /// <returns>200</returns> 
        [HttpGet("{song_id:int:min(1)}/parts/{Id:int:min(1)}")]
        public async Task<IActionResult> GetPartById(int Id)
        {
            var SuperObject = new SuperDTO(Id);
            var PartObject = await repository.GetPartById(SuperObject);
            if (PartObject != null)
                return base.Ok(PartObject);
            else
                return base.BadRequest("No song (or part) by that Id"); // TODO: tell if song or part missing
        }


        /// <summary>
        /// Create new part. Song must be id of some entity in DB.
        /// </summary>
        /// <param name="NewPartObject"></param>
        /// <param name="song_id"></param>
        /// <returns>201</returns>
        [HttpPost("{song_id:int:min(1)}/parts")]
        public async Task<IActionResult> CreatePart(int song_id, [FromBody] NewPartDTO NewPartObject)
        {
            if (song_id != NewPartObject.SongId)
                return base.BadRequest("Url Id must match SongId");

            var result = await repository.CreatePartCommand(NewPartObject, base.UserID);
            if (result != null)
                return base.Created($"api/songs/{song_id}/parts/{result.Id}", ""); 
            else
                return base.BadRequest("No song by that Id");
        }

        /// <summary>
        /// Update part. 
        /// </summary>
        /// <param name="UpdatePartObject"></param>
        /// <param name="song_id"></param>
        /// <returns>204</returns>
        [HttpPatch("{song_id:int:min(1)}/parts/{Id:int:min(1)}")]
        public async Task<IActionResult> UpdatePart(int song_id, [FromBody] UpdatePartDTO UpdatePartObject)
        {
            if (song_id != UpdatePartObject.SongId)
                return base.BadRequest("Url Id must match SongId");

            var result = await repository.UpdatePartCommand(UpdatePartObject, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("No song (or part) by that Id");
        }

        
        /// <summary>
        /// Delete Part by Id
        /// </summary>
        /// <returns>204</returns> 
        [HttpDelete("{song_id:int:min(1)}/parts/{Id:int:min(1)}")]
        public async Task<IActionResult> DeletePart(int Id)
        {
            var DeletePartObject = new SuperDTO(Id);
            bool result = await repository.DeletePart(DeletePartObject, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("No song (or part) by that Id");
        }

        
        #endregion
    }
}
