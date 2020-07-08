using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class SongRepository
    {

        private DissimilisDbContext _context;
        public SongRepository(DissimilisDbContext _context)
        {
            this._context = _context;
        }

        public async Task<Song[]> GetAllSongs(CancellationToken cancellationToken)
        {
            var SongModelArray = await this._context.Songs.ToArrayAsync(cancellationToken);
            return SongModelArray;
        }

        public async Task<Song> CreateSong(CreateSongCommand request, CancellationToken cancellation)
        {
            var SongObject = new Song()
            {
                Title = request.NewSongObject.Title,
                ArrangerId = 1  // TODO: Hardcoded for now. Should be set from request
            };
            await this._context.Songs.AddAsync(SongObject);
            await this._context.SaveChangesAsync();
            return SongObject;
        }
    }
}
