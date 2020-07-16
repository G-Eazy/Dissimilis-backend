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
    public class PartController : ControllerBase
    {
        private PartRepository _repository;
        
        public PartController(DissimilisDbContext context)
        {
            this._repository = new PartRepository(context);
        }
        #region CRUD Part
        /// <summary>
        /// Create new part. Song must be id of some entity in DB.
        /// </summary>
        /// <param name="NewPartDTO"></param>
        /// <returns>201</returns>
        [HttpPost("{song_id:int:min(1)}/parts")]
        public async Task<IActionResult> CreatePart(int song_id, [FromBody] NewPartDTO NewPartObject)
        {
            if (song_id != NewPartObject.SongId)
                return base.BadRequest("Url Id must match SongId");

            var result = await _repository.CreatePartCommand(NewPartObject);
            if (result != null)
                return base.Created($"api/songs/{song_id}/parts/{result.Id}", ""); 
            else
                return base.BadRequest("No song by that Id");
        }
        
        #endregion
    }
}
