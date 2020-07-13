using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoLogin
{
    public class LoginRepository
    {
        private DissimilisDbContext _context;
        public LoginRepository(DissimilisDbContext context)
        {
            this._context = context;
        }

        public async Task<User> GetAllSongs(CancellationToken cancellationToken)
        {
            var User = 
            return SongModelArray;
        }
    }
}
}
