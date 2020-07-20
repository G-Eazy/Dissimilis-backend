using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    interface INoteRepository
    {
        Task<NoteDTO> CreateNote(NewNoteDTO note, uint userId);
        Task<Note> FindNoteById(int Id);
        Task<bool> UpdateNote(NoteDTO noteObject, uint userId);
        Task<bool> DeleteNote(NoteDTO noteObject, uint userId);
        Task<bool> DeleteNoteById(int note_id, uint userId);
        bool ValidateUser(uint userId, Song bar);

    }
}
