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

        public async Task<SongBar> GetSongBarById(int songId, int partId, int barId, CancellationToken cancellationToken)
        {
            var bar = await Context.SongBars
                .Include(b => b.SongVoice.Song)
                .Include(b => b.Notes)
                .Where(b => b.SongVoice.SongId == songId && b.SongVoiceId == partId)
                .SingleOrDefaultAsync(x => x.Id == barId, cancellationToken);

            if (bar == null)
            {
                throw new NotFoundException($"Bar with ID {barId} not found.");
            }

            return bar;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
