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

        public async Task<NoteDTO> CreateNote(NewNoteDTO note, int barId, uint userId)
        {
            Note BarModel = new Note() { NoteNumber = note.NoteNumber, BarId = note.BarId, Length = note.Length, NoteValues = note.NoteValues };
            this.context.UserId = userId;
            await this.context.SaveChangesAsync();

            NoteDTO noteDTO = new NoteDTO() 
            { 
                Id = BarModel.Id, 
                BarId = BarModel.BarId, 
                Length = BarModel.Length, 
                NoteNumber = BarModel.NoteNumber, 
                NoteValues = BarModel.NoteValues 
            };

            return noteDTO;
        }

        public async Task<bool> DeleteNote(NoteDTO noteObject)
        {
            bool Deleted = false;
            Note deleteNote = await FindNoteById(noteObject.Id);
            this.context.Remove(deleteNote);
            
            //Check if any entries was changed, if yes it should have been deleted
            var entries = await this.context.SaveChangesAsync();
            if (entries > 0) Deleted = true;
            
            return Deleted;
        }

        public async Task<Note> FindNoteById(int Id)
        {
            return await this.context.Notes.SingleOrDefaultAsync(n => n.Id == Id);
        }

        public async Task<bool> UpdateNote(NoteDTO noteObject, int barId, uint userId)
        {
            bool Updated = false;
            Note nodeModel = await this.context.Notes.SingleOrDefaultAsync(n => n.Id == noteObject.Id);
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
    }
}
