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
        [HttpPost("{bar_id}")]
        public async Task<IActionResult> CreateBar([FromBody] NewBarDTO BarObject)
        {
            var result = await repository.CreateBar(BarObject, base.UserID);
            if (result is null)
                return base.BadRequest("No song by that Id");
            else
                return base.Created($"api/bars/{result.Id}", "");
        }

        /// <summary>
        /// Create new Bar.
        /// </summary>
        /// <param name="bar_id"></param>
        /// <returns>201</returns>
        [HttpGet("{bar_id}")]
        public async Task<IActionResult> GetBar(int bar_id)
        {
            var result = await repository.GetBar(bar_id, base.UserID);
            if (result is null)
                return base.BadRequest("No song by that Id");
            else
                return base.Created($"api/bars/{result.Id}", "");
        }

        /// <summary>
        /// Update the bar, taking in only the bar params
        /// </summary>
        /// <param name="BarObject"></param>
        /// <param name="bar_id"></param>
        /// <returns>201</returns>
        [HttpPatch("{bar_id}")]
        public async Task<IActionResult> UpdateBar(int bar_id, [FromBody] UpdateBarDTO BarObject)
        {
            var result = await repository.UpdateBar(BarObject, base.UserID);
            if (result is false)
                return base.BadRequest("Something went wrong updating the bar");

            return base.Created($"api/bars/{bar_id}", "Was created");
        }

        /// <summary>
        /// Delete a bar
        /// </summary>
        /// <param name="bar_id"></param>
        /// <returns>201</returns>
        [HttpDelete("{bar_id}")]
        public async Task<IActionResult> DeleteBar(int bar_id)
        {
            var result = await repository.DeleteBarById(bar_id, base.UserID);
            if (result is false)
                return base.BadRequest("Something went wrong updating the bar");

            return base.Created($"api/bars/{bar_id}", "Was created");
        }



        #endregion
    }
}
