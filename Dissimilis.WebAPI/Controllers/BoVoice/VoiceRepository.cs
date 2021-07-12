using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Exceptions;
using System.Collections.Generic;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class VoiceRepository
    {
        internal readonly DissimilisDbContext context;

        public VoiceRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<SongVoice> GetSongVoiceById(int songId, int songVoiceId, CancellationToken cancellationToken)
        {
            var part = await context.SongVoices
                .Include(sv => sv.Instrument)
                .Include(sv => sv.Song)
                .Where(p => p.SongId == songId)
                .FirstOrDefaultAsync(p => p.Id == songVoiceId, cancellationToken);

            if (part == null)
            {
                throw new NotFoundException($"Part with SongId {songId} and PartId {songVoiceId} not found.");
            }

            await context.SongBars
                .Include(sb => sb.Notes)
                .Where(sb => sb.SongVoiceId == songVoiceId)
                .LoadAsync(cancellationToken);

            return part;
        }

        public async Task<List<SongVoice>> MigrateSongVoice(CancellationToken cancellationToken)
        {
            var voices = await context.SongVoices
                .Include(v=> v.Instrument)
                .ToListAsync(cancellationToken);

            return voices;
        }

    }
}
