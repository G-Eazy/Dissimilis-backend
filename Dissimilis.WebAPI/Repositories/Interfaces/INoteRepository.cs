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
        Task<int> CreateNote(NewNoteDTO note, uint userId);
        Task<NoteDTO> GetNote(int Id);
        Task<bool> UpdateNote(UpdateNoteDTO noteObject, uint userId);
        Task<bool> DeleteNote(int note_id, uint userId);

    }
}
