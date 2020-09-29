using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
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
    }
}
