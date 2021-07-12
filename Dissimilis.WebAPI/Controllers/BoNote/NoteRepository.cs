using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoNote
{
    public class NoteRepository
    {
        internal readonly DissimilisDbContext context;

        public NoteRepository(DissimilisDbContext context)
        {
            this.context = context;
        }
        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        internal async Task<SongNote> GetSongNoteById(int songNoteId, CancellationToken cancellationToken)
        {
            var songNote = await context.SongNotes
                .FirstOrDefaultAsync(songNote => songNote.Id == songNoteId);

            if (songNote == null)
            {
                throw new NotFoundException($"Note with NoteId {songNoteId} not found.");
            }

            return songNote;
        }
    }
}
