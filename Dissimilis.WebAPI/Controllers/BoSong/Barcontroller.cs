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
    [Route("api/bars")]
    [ApiController]
    public class BarController : UserControllerBase
    {
        private BarRepository repository;

        public BarController(DissimilisDbContext context)
        {
            this.repository = new BarRepository(context);
        }

        #region CRUD bar
        /// <summary>
        /// Create new Bar.
        /// </summary>
        /// <param name="BarObject"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreateBar([FromBody] NewBarDTO BarObject)
        {
            var result = await repository.CreateBar(BarObject, base.UserID);
            if (result is 0)
                return base.BadRequest("No song by that Id");
            else
                return base.Created($"api/bars/{result}", $"{result}");
        }

        /// <summary>
        /// Get Bar by Id.
        /// </summary>
        /// <param name="barId"></param>
        /// <returns>201</returns>
        [HttpGet("{barId}")]
        public async Task<IActionResult> GetBar(int barId)
        {
            var result = await repository.GetBar(barId);
            if (result != null)
                return base.Ok(result);

            return base.BadRequest("No bar by that id");
        }

        /// <summary>
        /// Update the bar, taking in only the bar params
        /// </summary>
        /// <param name="BarObject"></param>
        /// <param name="barId"></param>
        /// <returns>201</returns>
        [HttpPatch("{barId}")]
        public async Task<IActionResult> UpdateBar(int barId, [FromBody] UpdateBarDTO BarObject)
        {
            var result = await repository.UpdateBar(BarObject, base.UserID);
            if (result)
                return base.Created($"api/bars/{barId}", $"{barId}");

            return base.BadRequest("Something went wrong updating the bar");
        }

        /// <summary>
        /// Delete a bar
        /// </summary>
        /// <param name="barId"></param>
        /// <returns>201</returns>
        [HttpDelete("{bar_id}")]
        public async Task<IActionResult> DeleteBar(int barId)
        {
            var result = await repository.DeleteBar(barId, base.UserID);
            if (result)
                return base.NoContent();

            return base.BadRequest("Something went wrong deleting the bar");
        }



        #endregion
    }
}
