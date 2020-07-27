using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Repositories.Interfaces;
using Dissimilis.WebAPI.Repositories.Validators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories
{
    public class NoteRepository : BaseRepository, INoteRepository
    {
        private readonly DissimilisDbContext context;
        
        public NoteRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get Note by Id
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public async Task<NoteDTO> GetNote (int noteId)
        {
            if (noteId <= 0) return null;

            Note NoteModel = await this.context.Notes
                .SingleOrDefaultAsync(x => x.Id == noteId);
            if (NoteModel is null) return null;

            NoteDTO NoteModelObject = new NoteDTO(NoteModel);
            return NoteModelObject;
        }

        /// <summary>
        /// Create a new note with the NewNoteDTO
        /// </summary>
        /// <param name="NewNoteObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CreateNote(NewNoteDTO NewNoteObject, uint userId)
        {
            if (! IsValidDTO<NewNoteDTO, NewNoteDTOValidator>(NewNoteObject)) return 0;

            Note CheckNoteNumber = await this.context.Notes
                .SingleOrDefaultAsync(b => b.NoteNumber == NewNoteObject.NoteNumber 
                                        && b.BarId == NewNoteObject.BarId);
            if (CheckNoteNumber != null)
                await UpdateNoteNumbers(CheckNoteNumber.NoteNumber, CheckNoteNumber.BarId, userId);

            Note NoteModel = new Note()
            {
                NoteNumber = NewNoteObject.NoteNumber,
                BarId = NewNoteObject.BarId,
                Length = NewNoteObject.Length,
                NoteValues = NewNoteObject.Notes
            };

            this.context.UserId = userId;
            await this.context.Notes.AddAsync(NoteModel);
            await this.context.TrySaveChangesAsync();

            return NoteModel.Id;
        }

        public async Task<bool> CreateAllNotes(int barId, NewNoteDTO[] noteObjects, uint userId)
        {
            if (noteObjects == null || noteObjects.Count() == 0) return false;
            if (barId is 0) return false;

            byte noteNumber = 1;

            foreach(NewNoteDTO note in noteObjects)
            {
                note.BarId = barId;
                note.NoteNumber = noteNumber++;
                if (note.Notes.Count() == 0) continue;
                int NoteCreated = await CreateNote(note, userId);
                if (NoteCreated == 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Update note numbers
        /// </summary>
        /// <param name="noteNumber"></param>
        /// <param name="barId"></param>
        /// <param name="userId"></param>
        private async Task<bool> UpdateNoteNumbers(int noteNumber, int barId, uint userId)
        {
            Note[] AllNotes = this.context.Notes.Where(b => b.BarId == barId)
                .OrderBy(x => x.NoteNumber)
                .ToArray();

            for (int i = noteNumber - 1; i < AllNotes.Count(); i++)
            {
                AllNotes[i].NoteNumber++;
            }

            this.context.UserId = userId;
            await this.context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Update the notes with the new values in NoteDTO
        /// </summary>
        /// <param name="UpdateNoteObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNote(UpdateNoteDTO UpdateNoteObject, uint userId)
        {
            bool Updated = false;

            if (! IsValidDTO<UpdateNoteDTO, UpdateNoteDTOValidator>(UpdateNoteObject)) return Updated;

            Note nodeModel = await this.context.Notes.Include(x => x.Bar)
                .ThenInclude(x => x.Part)
                .ThenInclude(x => x.Song)
                .SingleOrDefaultAsync(n => n.Id == UpdateNoteObject.Id);

            //Validate user if they are allowed to edit here
            if (nodeModel != null && ValidateUser(userId, nodeModel.Bar.Part.Song))
            {
                Note CheckNoteNumber = await this.context.Notes.SingleOrDefaultAsync(b => b.NoteNumber == UpdateNoteObject.NoteNumber && b.BarId == UpdateNoteObject.BarId);
                if (CheckNoteNumber != null)
                {
                    await UpdateNoteNumbers(UpdateNoteObject.NoteNumber, UpdateNoteObject.BarId, userId);
                }
                if (nodeModel.Length != UpdateNoteObject.Length) nodeModel.Length = UpdateNoteObject.Length;
                if (nodeModel.NoteValues != UpdateNoteObject.Notes) nodeModel.NoteValues = UpdateNoteObject.Notes;
                if (nodeModel.NoteNumber != UpdateNoteObject.NoteNumber) nodeModel.NoteNumber = UpdateNoteObject.NoteNumber;

                this.context.UserId = userId;
                Updated = await this.context.TrySaveChangesAsync();
            }

            return Updated;
        }
        
        /// <summary>
        /// Delete a note, using the ID 
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteNote(int noteId, uint userId)
        {
            bool Deleted = false;
            if (noteId <= 0) return Deleted;

            Note DeletedNote = await this.context.Notes
                .Include(n => n.Bar)
                .ThenInclude(n => n.Part)
                .ThenInclude(n => n.Song)
                .SingleOrDefaultAsync(n => n.Id == noteId);

            if (DeletedNote != null && ValidateUser(userId, DeletedNote.Bar.Part.Song))
            {
                this.context.Remove(DeletedNote);
                Deleted = await this.context.TrySaveChangesAsync();

            }

            return Deleted;
        }

    }
}
