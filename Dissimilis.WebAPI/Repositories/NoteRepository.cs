using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories
{
    class NoteRepository : INoteRepository
    {
        private readonly DissimilisDbContext context;
        
        public NoteRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Create a new note with the NewNoteDTO
        /// </summary>
        /// <param name="note"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<NoteDTO> CreateNote(NewNoteDTO note, uint userId)
        {
            Note NoteModel = new Note() { NoteNumber = note.NoteNumber, BarId = note.BarId, Length = note.Length, NoteValues = note.NoteValues };
            this.context.UserId = userId;
            await this.context.Notes.AddAsync(NoteModel);
            await this.context.TrySaveChangesAsync();

            if (!ValidateUser(userId, NoteModel.Bar.Part.Song)) return null;

            NoteDTO noteDTO = new NoteDTO() 
            { 
                Id = NoteModel.Id, 
                BarId = NoteModel.BarId, 
                Length = NoteModel.Length, 
                NoteNumber = NoteModel.NoteNumber, 
                NoteValues = NoteModel.NoteValues 
            };

            return noteDTO;
        }

        

        /// <summary>
        /// Delete a note, using the ID that is in NoteDTO
        /// </summary>
        /// <param name="noteObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteNote(NoteDTO noteObject, uint userId)
        {
            bool Deleted = false;
            Note deleteNote = await FindNoteById(noteObject.Id);

            if (!ValidateUser(userId, deleteNote.Bar.Part.Song)) return false;
            this.context.Remove(deleteNote);
            
            //Check if any entries was changed, if yes it should have been deleted
            var entries = await this.context.SaveChangesAsync();
            if (entries > 0) Deleted = true;
            
            return Deleted;
        }

        /// <summary>
        /// Delete a note, using the ID that is in NoteDTO
        /// </summary>
        /// <param name="note_id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteNoteById(int note_id, uint userId)
        {
            bool Deleted = false;
            Note deleteNote = await FindNoteById(note_id);

            //Check if the user is allowed to delete this notepart
            if (!ValidateUser(userId, deleteNote.Bar.Part.Song)) return false;

            this.context.Remove(deleteNote);

            //Check if any entries was changed, if yes it should have been deleted
            Deleted = await this.context.TrySaveChangesAsync();

            return Deleted;
        }

        /// <summary>
        /// Find a Note by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Note> FindNoteById(int Id)
        {
            return await this.context.Notes.SingleOrDefaultAsync(n => n.Id == Id);
        }

        /// <summary>
        /// Update the notes with the new values in NoteDTO
        /// </summary>
        /// <param name="noteObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNote(NoteDTO noteObject, uint userId)
        {
            bool Updated = false;

            if (noteObject is null) return false;
            Note nodeModel = await this.context.Notes.SingleOrDefaultAsync(n => n.Id == noteObject.Id);

            //Validate user if they are allowed to edit here
            if (!ValidateUser(userId, nodeModel.Bar.Part.Song)) return false;

            if (nodeModel is null) throw new Exception("The note with Id: " + noteObject.Id + " does not exist");

            if(nodeModel.Length != noteObject.Length)
                nodeModel.Length = noteObject.Length;

            if(nodeModel.NoteValues != noteObject.NoteValues)
                nodeModel.NoteValues = noteObject.NoteValues;

            if (nodeModel.NoteNumber != noteObject.NoteNumber)
                nodeModel.NoteNumber = noteObject.NoteNumber;

            this.context.UserId = userId;
            var entries = await this.context.SaveChangesAsync();
            //If more than 0 entires was saved/updated set Updated to true
            if(entries > 0) Updated = true;

            return Updated;
        }

        /// <summary>
        /// Check if the user belongs to the bar it is trying to access/edit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        public bool ValidateUser(uint userId, Song song)
        {
            try
            {
                if (userId == song.CreatedById)
                    return true;
                return false;
            }
            catch
            {
                throw new ArgumentException("The user is not allowed to edit on this song");
            }
        }
    }
}
