using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoUser
{
    public class UserRepository
    {

        private DissimilisDbContext _context;
        public UserRepository(DissimilisDbContext _context)
        {
            this._context = _context;
        }

        public async Task<User[]> GetAllUsers(CancellationToken cancellationToken)
        {
            var UserModelArray = await this._context.Users.ToArrayAsync(cancellationToken);
            return UserModelArray;
        }

        public async Task<User> GetUserById(int UserId, CancellationToken cancellationToken)
        {
            return await this._context.Users
                .FirstOrDefaultAsync(u => u.Id == UserId, cancellationToken);
        }
        
    }
}
