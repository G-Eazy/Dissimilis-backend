using System;
using Dissimilis.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;


namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class Repository
    {
        private readonly DissimilisDbContext context;

        public Repository(DissimilisDbContext context)
        {
            this.context = context;
        }


        public async Task<SongBar> GetSongBarById(int songId, int partId, int barId, CancellationToken cancellationToken)
        {
            var bar = await context.SongBars
                .Include(b => b.SongVoice)
                .Include(b => b.Notes)
                .Where(b => b.SongVoice.SongId == songId && b.SongVoiceId == partId)
                .FirstOrDefaultAsync(x => x.Id == barId, cancellationToken);

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

        public async Task<SongVoice> GetSongPartById(int songId, int partId, CancellationToken cancellationToken)
        {
            var part = await context.SongVoices
                .Include(p => p.SongBars)
                .Where(p => p.SongId == songId)
                .FirstOrDefaultAsync(p => p.Id == partId, cancellationToken);

            if (part == null)
            {
                throw new NotFoundException($"Part with SongId {songId} and PartId {partId} not found.");
            }

            return part;
        }

        /// <summary>
        /// Looks for an instrument with title InstrumentName, and creates if non-existant
        /// </summary>
        public async Task<Instrument> CreateOrFindInstrument(string InstrumentName, CancellationToken cancellationToken)
        {

            var instrument = await this.context.Instruments
                .FirstOrDefaultAsync(i => i.Name == InstrumentName, cancellationToken: cancellationToken);

            if (instrument != null)
            {
                return instrument;
            }

            instrument = new Instrument(InstrumentName);

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

        public async Task<SongVoice> GetSongVoiceById(int songId, int songVoiceId, CancellationToken cancellationToken)
        {
            var songVoice = await context.SongVoices
                .Include(sv => sv.SongBars)
                .FirstOrDefaultAsync(sv => sv.SongId == songId && sv.Id == songVoiceId, cancellationToken);

            if (songVoice == null)
            {
                throw new NotFoundException($"SongVoice with Id {songVoiceId} not found.");
            }

            await context.SongNotes
                .Where(sn => sn.SongBar.SongVoiceId == songVoiceId)
                .LoadAsync(cancellationToken);

            return songVoice;
        }
    }
}
