using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    interface INoteReposetory
    {
        Task<NotesDTO> CreateNote(NewNoteDTO note, int barId);
        Task<Note> FindNoteById(int Id);
        Task<Note> FindNoteByPriority(int priority);
        Task<bool> UpdateNoteById(int Id);

    }
}
