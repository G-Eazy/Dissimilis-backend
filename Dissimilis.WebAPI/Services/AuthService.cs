using System;
using System.Linq;
using System.Text.RegularExpressions;
using Dissimilis.Configuration;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Dissimilis.WebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly DissimilisDbContext _dbContext;

        internal const string USERID_HEADER_NAME = "X-User-ID";
        private const int TestUserId = -99;

        public AuthService(IHttpContextAccessor httpContext, DissimilisDbContext dbContext)
        {
            _httpContext = httpContext;
            _dbContext = dbContext;
        }

        public int? GetCurrentUserId()
        {
            var val = GetHeaderValue(USERID_HEADER_NAME);

            if (val != null && Regex.IsMatch(val, @"^[\d]+$"))
            {
                return int.Parse(val);
            }

            if (ConfigurationInfo.IsLocalDebugBuild())
            {
                return 1;
            }

            return null;
        }

        public User GetVerifiedCurrentUser()
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new NotFoundException("UserId not found");
            }

            var user = _dbContext.Users.SingleOrDefault(u => u.Id == currentUserId.Value);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return user;
        }

        private string GetHeaderValue(string key)
        {
            var headers = _httpContext?.HttpContext?.Request.Headers;

            if (headers == null)
            {
                return null;
            }

            if (headers.Any(x => x.Key.ToLower() == key.ToLower()))
            {
                return headers[key];
            }

            return null;
        }
    }
}
