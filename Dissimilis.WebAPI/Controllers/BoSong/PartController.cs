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
    [Route("api/voice")]
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
        /// Create voice
        /// </summary>
        /// <param name="NewPartObject"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePart([FromBody] NewPartDTO NewPartObject)
        {
            var result = await repository.CreatePart(NewPartObject, base.UserID);

            if (result != 0)
                return base.Created($"api/voice/{result}", $"{result}"); 
            else
                return base.BadRequest("Unable to create voice");
        }

        /// <summary>
        /// Get voice
        /// </summary>
        /// <returns>200</returns> 
        [HttpGet]
        public async Task<IActionResult> GetPart([FromQuery] int voiceId)
        {
            var PartObject = await repository.GetPart(voiceId);
            if (PartObject != null)
                return base.Ok(PartObject);
            else
                return base.NotFound();
        }

        /// <summary>
        /// Update voice
        /// </summary>
        /// <param name="UpdatePartObject"></param>
        /// <returns>204</returns>
        [HttpPatch]
        public async Task<IActionResult> UpdatePart([FromBody] UpdatePartDTO UpdatePartObject)
        {
            var result = await repository.UpdatePart(UpdatePartObject, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("Unable to update Voice");
        }
        
        /// <summary>
        /// Delete voice
        /// </summary>
        /// <returns>204</returns> 
        [HttpDelete]
        public async Task<IActionResult> DeletePart([FromQuery] int voiceId)
        {
            bool result = await repository.DeletePart(voiceId, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("Unable to delete Voice");
        }

        
        #endregion
    }
}
