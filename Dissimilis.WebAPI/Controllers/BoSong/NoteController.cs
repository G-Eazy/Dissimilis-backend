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
        /// Get Note by Id
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNote([FromQuery] int noteId)
        {
            var NoteObject = await repository.GetNote(noteId);
            if (NoteObject != null)
                return base.Ok(NoteObject);
            else
                return base.BadRequest("There was no note with this ID");
        }

        /// <summary>
        /// Create new Note using NewNoteDTO
        /// </summary>
        /// <param name="NewNoteObject"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NewNoteDTO NewNoteObject)
        {
            var result = await repository.CreateNote(NewNoteObject, base.UserID);
            if (result != 0)
                return base.Created($"api/note/{result}", $"{result}");
            else
                return base.BadRequest("No song by that Id");
        }

        /// <summary>
        /// Update note by using NoteDTO
        /// </summary>
        /// <param name="NoteObject"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateNote([FromBody] UpdateNoteDTO NoteObject)
        {
            var result = await repository.UpdateNote(NoteObject, base.UserID);
            if (result)
                return base.NoContent();

            return base.BadRequest("No song by that Id");
        }


        /// <summary>
        /// Delete a note by Id
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteNote([FromQuery]int noteId)
        {
            var respone = await repository.DeleteNote(noteId, base.UserID);
            if(respone != false)
                return base.NoContent();

            return base.BadRequest("No note was deleted");
        }

        #endregion
    }
}

