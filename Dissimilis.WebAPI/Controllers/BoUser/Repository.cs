using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoUser
{
    public class Repository
    {

        private DissimilisDbContext _context;
        public Repository(DissimilisDbContext context)
        {
            _context = context;
        }

        public async Task<User[]> GetAllUsers(CancellationToken cancellationToken)
        {
            return await _context.Users
                .OrderBy(u => u.Name)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<User> GetUserById(int userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException($"User with Id {userId} not found");
            }
            return user;
        }
    }
}
