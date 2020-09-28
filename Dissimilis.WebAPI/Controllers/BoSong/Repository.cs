using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class Repository
    {

        private DissimilisDbContext _context;
        public Repository(DissimilisDbContext context)
        {
            _context = context;
        }


        public async Task<Song> GetSongById(int songId, CancellationToken cancellationToken)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            await _context.SongVoices
                .Include(p => p.Instrument)
                .Where(p => p.SongId == songId)
                .LoadAsync(cancellationToken);

            await _context.SongBars
                .Include(b => b.Notes)
                .Where(b => b.SongVoice.SongId == songId)
                .LoadAsync(cancellationToken);

            return song;
        }

        public async Task SaveAsync(Song song, CancellationToken cancellationToken)
        {
            await _context.Songs.AddAsync(song, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Song> GetSongByIdForUpdate(int songId, CancellationToken cancellationToken)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            return song;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSong(Song song, CancellationToken cancellationToken)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Song[]> GetSongSearchList(SearchQueryDto searchCommand, CancellationToken cancellationToken)
        {

            var query = _context.Songs
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchCommand.Title))
            {
                var textSearch = $"%{searchCommand.Title.Trim()}%";
                query = query
                    .Where(s => EF.Functions.Like(s.Title, textSearch))
                    .AsQueryable();
            }

            if (searchCommand.ArrangerId != null)
            {
                query = query
                    .Where(s => s.ArrangerId == searchCommand.ArrangerId)
                    .AsQueryable();
            }

            if (searchCommand.OrderByDateTime)
            {
                query = query
                    .OrderByDescending(s => s.UpdatedOn);
            }
            else
            {
                query = query
                    .OrderBy(s => s.Title);
            }


            if (searchCommand.Num != null)
            {
                query = query
                    .Take(searchCommand.Num.Value)
                    .AsQueryable();
            }

            var result = await query
                .Include(s => s.Arranger)
                .ToArrayAsync(cancellationToken);

            return result;
        }
    }
}
