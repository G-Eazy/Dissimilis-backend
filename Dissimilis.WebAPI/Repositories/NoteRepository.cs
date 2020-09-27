using Dissimilis.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;

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
        public async Task<NoteDto> GetNote (int noteId)
        {
            if (noteId <= 0) return null;

            SongNote songNoteModel = await this.context.SongNotes
                .SingleOrDefaultAsync(x => x.Id == noteId);
            if (songNoteModel is null) return null;

            NoteDto NoteModelObject = new NoteDto(songNoteModel);
            return NoteModelObject;
        }

        /// <summary>
        /// Create a new note with the NewNoteDTO
        /// </summary>
        /// <param name="NewNoteObject"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> CreateNote(NoteDto NewNoteObject, uint userId)
        {
            if (! IsValidDTO<NoteDto, NewNoteDTOValidator>(NewNoteObject)) return 0;

            string[] newNote = NewNoteObject.Notes;
            if (newNote.Count() == 0) {
                 newNote = new string[1] { " " };
            }

            SongNote songNoteModel = new SongNote()
            {
                NoteNumber = NewNoteObject.NoteNumber,
                BarId = NewNoteObject.BarId,
                Length = NewNoteObject.Length,
                NoteValues = newNote
            };

            await this.context.SongNotes.AddAsync(songNoteModel);

            return songNoteModel.Id;
        }

        public async Task<bool> CreateAllNotes(int barId, NoteDto[] noteObjects, uint userId)
        {
            if (noteObjects == null || noteObjects.Count() == 0) return false;

            byte noteNumber = 1;

            foreach(NoteDto note in noteObjects)
            {
                note.BarId = barId;
                note.NoteNumber = noteNumber++;
                if (note.Notes.Count() == 0) continue;
                await CreateNote(note, userId);
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
            SongNote[] AllNotes = this.context.SongNotes.Where(b => b.BarId == barId)
                .OrderBy(x => x.NoteNumber)
                .ToArray();

            for (int i = noteNumber - 1; i < AllNotes.Count(); i++)
            {
                AllNotes[i].NoteNumber++;
            }

            
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

            SongNote nodeModel = await this.context.SongNotes.Include(x => x.SongBar)
                .ThenInclude(x => x.SongVoice)
                .ThenInclude(x => x.Song)
                .SingleOrDefaultAsync(n => n.Id == UpdateNoteObject.Id);

            //Validate user if they are allowed to edit here
            if (nodeModel != null && ValidateUser(userId, nodeModel.SongBar.SongVoice.Song))
            {
                SongNote checkSongNoteNumber = await this.context.SongNotes.SingleOrDefaultAsync(b => b.NoteNumber == UpdateNoteObject.NoteNumber && b.BarId == UpdateNoteObject.BarId);
                if (checkSongNoteNumber != null)
                {
                    await UpdateNoteNumbers(UpdateNoteObject.NoteNumber, UpdateNoteObject.BarId, userId);
                }
                if (nodeModel.Length != UpdateNoteObject.Length) nodeModel.Length = UpdateNoteObject.Length;
                if (nodeModel.NoteValues != UpdateNoteObject.Notes) nodeModel.NoteValues = UpdateNoteObject.Notes;
                if (nodeModel.NoteNumber != UpdateNoteObject.NoteNumber) nodeModel.NoteNumber = UpdateNoteObject.NoteNumber;

                
                 await this.context.SaveChangesAsync();
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

            SongNote deletedSongNote = await this.context.SongNotes
                .Include(n => n.SongBar)
                .ThenInclude(n => n.SongVoice)
                .ThenInclude(n => n.Song)
                .SingleOrDefaultAsync(n => n.Id == noteId);

            if (deletedSongNote != null && ValidateUser(userId, deletedSongNote.SongBar.SongVoice.Song))
            {
                this.context.Remove(deletedSongNote);
                await this.context.SaveChangesAsync();

            }

            return Deleted;
        }

    }
}
