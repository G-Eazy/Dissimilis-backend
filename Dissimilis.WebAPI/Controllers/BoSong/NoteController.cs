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
    [Route("api/note")]
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
        /// Create note
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
                return base.BadRequest("Unable to create Note");
        }
        
        /// <summary>
        /// Get note
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [HttpGet("{noteId:int:min(1)}")]
        public async Task<IActionResult> GetNote(int noteId)
        {
            var NoteObject = await repository.GetNote(noteId);
            if (NoteObject != null)
                return base.Ok(NoteObject);
            else
                return base.NotFound();
        }

        /// <summary>
        /// Update note
        /// </summary>
        /// <param name="NoteObject"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateNote([FromBody] UpdateNoteDTO NoteObject)
        {
            var result = await repository.UpdateNote(NoteObject, base.UserID);
            if (result)
                return base.NoContent();

            return base.BadRequest("Unable to update Note");
        }

        /// <summary>
        /// Delete note
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [HttpDelete("{noteId:int:min(1)}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var result = await repository.DeleteNote(noteId, base.UserID);
            if(result)
                return base.NoContent();

            return base.BadRequest("Unable to delete Note");
        }

        #endregion
    }
}

