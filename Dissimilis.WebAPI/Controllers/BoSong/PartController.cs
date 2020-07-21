﻿using System;
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
    [Route("api/parts")]
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
        [HttpGet("{partId:int:min(1)}")]
        public async Task<IActionResult> GetPart([FromQuery] int partId)
        {
            var PartObject = await repository.GetPart(partId);
            if (PartObject != null)
                return base.Ok(PartObject);
            else
                return base.BadRequest("No song (or part) by that Id"); 
        }


        /// <summary>
        /// Create new part
        /// </summary>
        /// <param name="NewPartObject"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePart([FromBody] NewPartDTO NewPartObject)
        {
            var result = await repository.CreatePart(NewPartObject, base.UserID);

            if (result != 0)
                return base.Created($"api/parts/{result}", $"{result}"); 
            else
                return base.BadRequest("No song by that Id");
        }

        /// <summary>
        /// Update part by using UpdatePartDTO 
        /// </summary>
        /// <param name="UpdatePartObject"></param>
        /// <returns>204</returns>
        [HttpPatch("{partId:int:min(1)}")]
        public async Task<IActionResult> UpdatePart([FromBody] UpdatePartDTO UpdatePartObject)
        {
            var result = await repository.UpdatePart(UpdatePartObject, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("No song (or part) by that Id");
        }

        
        /// <summary>
        /// Delete Part by Id
        /// </summary>
        /// <returns>204</returns> 
        [HttpDelete("{partId:int:min(1)}")]
        public async Task<IActionResult> DeletePart([FromQuery] int partId)
        {
            bool result = await repository.DeletePart(partId, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("No song (or part) by that Id");
        }

        
        #endregion
    }
}
