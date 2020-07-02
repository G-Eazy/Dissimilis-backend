using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Audit
{
    public interface IUserService
    {
        string GetUserId();
        string GetUserName();
    }
}
