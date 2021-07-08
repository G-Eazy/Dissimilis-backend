using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Extensions.Models;
using System;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class Repository
    {

        internal DissimilisDbContext Context;
        public Repository(DissimilisDbContext context)
        {
            Context = context;
        }


        public async Task<Song> GetFullSongById(int songId, CancellationToken cancellationToken)
        {
            var song = await Context.Songs
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            await Context.SongVoices
                .Include(p => p.Instrument)
                .Where(p => p.SongId == songId)
                .LoadAsync(cancellationToken);

            await Context.SongBars
                .Include(b => b.Notes)
                .Where(b => b.SongVoice.SongId == songId)
                .LoadAsync(cancellationToken);

            return song;
        }

        public async Task<Song[]> GetAllSongsInMyLibrary(int userId, CancellationToken cancellationToken)
        {
            var songs = await Context.Songs
                .Where(s => s.CreatedById == userId || s.ArrangerId == userId)
                .ToArrayAsync(cancellationToken);
            
            return songs;
        }

        public async Task SaveAsync(Song song, CancellationToken cancellationToken)
        {
            await Context.Songs.AddAsync(song, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Song> GetSongByIdForUpdate(int songId, CancellationToken cancellationToken)
        {
            var song = await Context.Songs
                .FirstOrDefaultAsync(s => s.Id == songId, cancellationToken);

            if (song == null)
            {
                throw new NotFoundException($"Song with Id {songId} not found");
            }

            return song;
        }

        public async Task UpdateAsync(Song song, User user, CancellationToken cancellationToken)
        {
            Console.WriteLine($"UpdateAsync with objects used:\n{song}\n{user}");
            Song oldSong = (Song)Context.Entry(song).OriginalValues.ToObject();
            song.PerformSnapshot(oldSong, user);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSong(Song song, CancellationToken cancellationToken)
        {
            Context.Songs.Remove(song);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Song[]> GetSongSearchList(SearchQueryDto searchCommand, CancellationToken cancellationToken)
        {

            var query = Context.Songs
                .Include(s => s.Arranger)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchCommand.Title))
            {
                var textSearch = $"%{searchCommand.Title.Trim()}%";
                query = query
                    .Where(s => EF.Functions.Like(s.Title, textSearch) || EF.Functions.Like(s.Arranger.Name, textSearch))
                    .AsQueryable();
            }

            if (searchCommand.ArrangerId != null)
            {
                query = query
                    .Where(s => s.ArrangerId == searchCommand.ArrangerId)
                    .AsQueryable();
            }

            if (searchCommand.OrderBy == "song")
            {
                query = searchCommand.OrderDescending ?
                    query.OrderBy(s => s.Title)
                    .ThenByDescending(s => s.UpdatedOn) :
                    query.OrderByDescending(s => s.Title)
                    .ThenByDescending(s => s.UpdatedOn);
            }
            else if (searchCommand.OrderBy == "user")
            {
                query = searchCommand.OrderDescending ?
                    query.OrderBy(s => s.Arranger.Name)
                    .ThenByDescending(s => s.UpdatedOn) :
                    query.OrderByDescending(s => s.Arranger.Name)
                    .ThenByDescending(s => s.UpdatedOn);
            }
            else
            {
                query = searchCommand.OrderDescending ? 
                    query.OrderByDescending(s => s.UpdatedOn) :
                    query.OrderBy(s => s.UpdatedOn);
            }

            if (searchCommand.Num != null)
            {
                query = query
                    .Take(searchCommand.Num.Value)
                    .AsQueryable();
            }

            var result = await query
                .ToArrayAsync(cancellationToken);

            return result;
        }
    }
}
