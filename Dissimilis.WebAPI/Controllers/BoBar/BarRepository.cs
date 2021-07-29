using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Exceptions;

namespace Dissimilis.WebAPI.Controllers.BoBar
{
    public class BarRepository
    {
        internal DissimilisDbContext Context;
        public BarRepository(DissimilisDbContext context)
        {
            Context = context;
        }

        public async Task<SongBar> GetSongBarById(int songId, int voiceId, int barId, CancellationToken cancellationToken)
        {
            var bar = await Context.SongBars
                .Include(b => b.SongVoice.Song)
                .Include(b => b.Notes)
                .Where(b => b.SongVoice.SongId == songId && b.SongVoiceId == voiceId)
                .SingleOrDefaultAsync(x => x.Id == barId, cancellationToken);

            if (bar == null) throw new NotFoundException($"Bar with ID {barId} not found.");

            return bar;
        }

        public async Task<SongBar> GetSongBarByPosition(int songId, int songVoiceId, int position, CancellationToken cancellationToken)
        {
            var bar = await Context.SongBars
                .Include(bar => bar.SongVoice.Song)
                .Include(bar => bar.Notes)
                .Where(bar => bar.SongVoice.SongId == songId && bar.SongVoiceId == songVoiceId)
                .SingleOrDefaultAsync(bar => bar.Position == position, cancellationToken);

            if (bar == null) throw new NotFoundException($"Bar with position {position} not found.");

            return bar;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
