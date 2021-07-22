using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Services
{
    public interface IPermissionCheckerService
    {
        Task<bool> CheckPermission(Organisation org, User user, Operation op, CancellationToken c);
        Task<bool> CheckPermission(Group group, User user, Operation op, CancellationToken c);
        bool CheckCreateOrganisationPermission(User user);
    }
}
