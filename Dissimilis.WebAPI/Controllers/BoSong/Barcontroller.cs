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
            if (result != 0)
                return base.Created($"api/bars?barsId={result}", $"{result}");

            return base.BadRequest("Couldn't create the bar");
        }

        /// <summary>
        /// Get Bar by Id.
        /// </summary>
        /// <param name="barId"></param>
        /// <returns>201</returns>
        [HttpGet]
        public async Task<IActionResult> GetBar([FromQuery]int barId)
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
        /// <returns>201</returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateBar([FromBody] UpdateBarDTO BarObject)
        {
            var result = await repository.UpdateBar(BarObject, base.UserID);
            if (result)
                return base.NoContent();

            return base.BadRequest("Something went wrong updating the bar");
        }

        /// <summary>
        /// Delete a bar
        /// </summary>
        /// <param name="barId"></param>
        /// <returns>201</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteBar([FromQuery] int barId)
        {
            var result = await repository.DeleteBar(barId, base.UserID);
            if (result)
                return base.NoContent();

            return base.BadRequest("Something went wrong deleting the bar");
        }



        #endregion
    }
}
