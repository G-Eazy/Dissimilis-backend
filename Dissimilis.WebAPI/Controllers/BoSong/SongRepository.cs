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
    }
}
