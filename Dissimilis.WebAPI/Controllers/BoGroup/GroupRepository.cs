using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup
{
    public class GroupRepository
    {
        internal DissimilisDbContext Context;
        public GroupRepository(DissimilisDbContext context)
        {
            Context = context;
        }

        public async Task SaveGroupAsync(Group group, CancellationToken cancellationToken)
        {
            await Context.Groups.AddAsync(group, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
