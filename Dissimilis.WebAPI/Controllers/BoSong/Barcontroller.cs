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
    public class Barcontroller : UserControllerBase
    {
        private BarRepository repository;

        public Barcontroller(DissimilisDbContext context)
        {
            this.repository = new BarRepository(context);
        }

        #region CRUD bar
        /// <summary>
        /// Create new Bar. We also need the par Id that it is related too.
        /// </summary>
        /// <param name="BarObject"></param>
        /// <param name="partId"></param>
        /// <returns>201</returns>
        [HttpPost("{song_id:int:min(1)}/parts/{part_id:int:min(1)}/bars")]
        public async Task<IActionResult> CreateBar(int partId, [FromBody] NewBarDTO BarObject)
        {
            if (partId != BarObject.PartId)
                return base.BadRequest("Url Id must match SongId");

            var result = await repository.CreateBar(BarObject, partId, base.UserID);
            if (result is null)
                return base.BadRequest("No song by that Id");
            else
                return base.Created($"api/songs/{partId}/parts/{result.Id}/bars", "");
            
        }

        #endregion
    }
}
