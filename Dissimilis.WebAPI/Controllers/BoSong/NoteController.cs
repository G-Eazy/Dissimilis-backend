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
    [Route("api/notes")]
    [ApiController]
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
        [HttpPost("{bar_id}")]
        public async Task<IActionResult> CreateNote(int bar_id, [FromBody] NewNoteDTO NewNoteObject)
        {
            if (bar_id != NewNoteObject.BarId)
                return base.BadRequest("Url Id must match SongId");

            var result = await repository.CreateNote(NewNoteObject, base.UserID);
            if (result != null)
                return base.Created($"api/note/{result.Id}", "");
            else
                return base.BadRequest("No song by that Id");
        }


        [HttpPatch("{note_id}")]
        public async Task<IActionResult> UpdateNote(int note_id, [FromBody] NoteDTO NoteObject)
        {
            if (note_id != NoteObject.Id)
                return base.BadRequest("Url Id must match SongId");

            var result = await repository.UpdateNote(NoteObject, base.UserID);
            if (result is false)
                return base.BadRequest("No song by that Id");

            return base.Ok($"Note with id: " + note_id + "was updated");
        }


        /// <summary>
        /// Delete a note using it's unique id
        /// </summary>
        /// <param name="note_id"></param>
        /// <returns></returns>
        [HttpDelete("{note_id}")]
        public async Task<IActionResult> DeleteNote(int note_id)
        {
            var respone = await repository.DeleteNoteById(note_id, base.UserID);
            if(respone is false)
            {
                return base.BadRequest("No note was deleted");
            }

            return base.Ok("note with id: " + note_id + " was deleted");
        }

        #endregion
    }
}

