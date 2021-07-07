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
                .FirstOrDefaultAsync(x => x.Id == barId, cancellationToken);

            if (bar == null)
            {
                throw new NotFoundException($"Bar with ID {barId} not found.");
            }

            return bar;
        }


        public async Task UpdateAsync(Song song, User user, CancellationToken cancellationToken)
        {
            song.PerformSnapshot(user);
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

        /// <summary>
        /// Looks for an instrument with title InstrumentName, and creates if non-existant
        /// </summary>
        public async Task<Instrument> CreateOrFindInstrument(string instrumentName, CancellationToken cancellationToken)
        {

            var instrument = await context.Instruments.FirstOrDefaultAsync(i => i.Name == instrumentName, cancellationToken);

            if (instrument != null)
            {
                return instrument;
            }

            if (instrumentName == null)
            {
                var mainDuplicate = await context.Instruments.FirstOrDefaultAsync(i => i.Name == "Main 1", cancellationToken);
                if (mainDuplicate != null)
                {
                    return mainDuplicate;
                }
                instrument = new Instrument("Main 1");
            }
            else
            {
                instrument = new Instrument(instrumentName);
            }

            await context.Instruments.AddAsync(instrument, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return instrument;
        }

        public async Task<Song> GetSongById(int songId, CancellationToken cancellationToken)
        {
            var song = await context.Songs
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

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
