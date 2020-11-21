using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Services
{
    public interface IAuthService
    {
        int? GetCurrentUserId();
        User GetVerifiedCurrentUser();
        
    }
}