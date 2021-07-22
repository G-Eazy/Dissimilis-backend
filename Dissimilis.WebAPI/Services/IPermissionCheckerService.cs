using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Services
{
    public interface _IPermissionCheckerService
    {
        Task<bool> CheckPermission(Organisation org, User user, Operation op, CancellationToken c);
        Task<bool> CheckPermission(Group group, User user, Operation op, CancellationToken c);
        Task<bool> CheckPermission(Song song, User user, Operation op, CancellationToken c);
        Task<bool> CheckTagPermission(Song song, User user, Operation op, Organisation org, Group? group, CancellationToken c);
    }
}
