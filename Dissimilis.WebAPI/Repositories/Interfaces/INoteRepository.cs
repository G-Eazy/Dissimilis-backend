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
        Task<NoteDTO> CreateNote(NewNoteDTO note, int barId, uint userId);
        Task<Note> FindNoteById(int Id);
        Task<bool> UpdateNote(NoteDTO noteObject, int BarId, uint userId);
        Task<bool> DeleteNote(NoteDTO noteObject);

    }
}
