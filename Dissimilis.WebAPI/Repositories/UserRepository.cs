using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using System;

namespace Dissimilis.WebAPI.Repositories
{
    public class UserRepository
    {
        private readonly DissimilisDbContext _context;
        public UserRepository(DissimilisDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateOrFindUserAsync(UserEntityMetadata userMeta)
        {
            var user = await GetUserByMsIdAsync(userMeta.id);
            if (user != null)
            {
                return user;
            }

            user = await GetUserByEmailAsync(userMeta.Email()) ?? await CreateUserAsync(userMeta);

            return user;
        }

        public async Task<User> CreateUserAsync(UserEntityMetadata meta)
        {
            var user = new User()
            {
                Name = meta.displayName,
                Email = (meta.mail ?? meta.userPrincipalName).ToLower(),
                MsId = meta.id
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Get user by give email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }




        /// <summary>
        /// Get user by the microsoft ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetUserByMsIdAsync(string id)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.MsId == id);
        }


        /// <summary>
        /// Update the user country fk
        /// </summary>
        /// <param name="user"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<User> UpdateUserCountryAsync(User user, Country country)
        {
            user.CountryId = country?.Id;
            
            await this._context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Update the user organisation fk
        /// </summary>
        /// <param name="user"></param>
        /// <param name="organisation"></param>
        /// <returns></returns>
        public async Task<User> UpdateUserOrganisationAsync(User user, Organisation organisation)
        {
            user.OrganisationId = organisation?.Id;
            
            await this._context.SaveChangesAsync();
            return user;
        }
    }
}
