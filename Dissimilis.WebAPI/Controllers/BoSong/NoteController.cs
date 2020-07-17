using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Repositories;
using Experis.Ciber.Web.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class NoteController : UserControllerBase
    {
        private NoteRepository repository;

        public NoteController(DissimilisDbContext context)
        {
            this.repository = new NoteRepository(context);
        }

        #region CRUD Part
        /// <summary>
        /// Create new Note. The Bar Id must also be provided for the associated Bar
        /// </summary>
        /// <param name="NewNoteObject"></param>
        /// <param name="bar_id"></param>
        /// <returns>201</returns>
        [HttpPost("{song_id:int:min(1)}/parts")]
        public async Task<IActionResult> CreateNote(int bar_id, [FromBody] NewNoteDTO NewNoteObject)
        {
            if (bar_id != NewNoteObject.BarId)
                return base.BadRequest("Url Id must match SongId");

            var result = await repository.CreateNote(NewNoteObject, bar_id, base.UserID);
            if (result != null)
                return base.Created($"api/songs/parts/{bar_id}/notes/{result.Id}", "");
            else
                return base.BadRequest("No song by that Id");
        }

        #endregion
    }
}

