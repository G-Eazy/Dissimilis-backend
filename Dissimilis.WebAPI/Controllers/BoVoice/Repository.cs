using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Exceptions;
using System.Collections.Generic;
using Dissimilis.WebAPI.Extensions.Models;


namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class Repository
    {
        internal readonly DissimilisDbContext context;

        public Repository(DissimilisDbContext context)
        {
            this.context = context;
        }


        public async Task<SongBar> GetSongBarById(int songId, int partId, int barId, CancellationToken cancellationToken)
        {
            var bar = await context.SongBars
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
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<SongVoice> GetSongVoiceById(int songId, int songVoiceId, CancellationToken cancellationToken)
        {
            var part = await context.SongVoices
                .Include(sv => sv.Instrument)
                .Include(sv => sv.Song)
                .Where(p => p.SongId == songId)
                .SingleOrDefaultAsync(p => p.Id == songVoiceId, cancellationToken);

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

        public async Task<Song> GetSongById(int songId, CancellationToken cancellationToken)
        {
            var song = await context.Songs
                .SingleOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            await context.SongVoices
                .Include(sv => sv.Instrument)
                .Where(sv => sv.SongId == songId)
                .LoadAsync(cancellationToken);

            await context.SongBars
                .Include(sb => sb.Notes)
                .Where(sb => sb.SongVoice.SongId == songId)
                .LoadAsync(cancellationToken);

            await context.SongSnapshots
                .Include(s => s.Song)
                .Where(s => s.SongId == songId)
                .LoadAsync(cancellationToken);

            return song;
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
