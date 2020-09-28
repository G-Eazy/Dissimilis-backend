using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.DTOs;

namespace Dissimilis.WebAPI.Repositories
{
    public class UserRepository 
    {
        private readonly DissimilisDbContext _context;
        public UserRepository(DissimilisDbContext context)
        {
            this._context = context;
        }

        public async Task<User> CreateOrFindUserAsync(UserEntityMetadata userMeta)
        {
            var user = await this.GetUserByMsIdAsync(userMeta.id);
            if(user is null)
            {
                user = await this.GetUserByEmailAsync(userMeta.Email());
                if(user is null)
                {
                    user = await CreateUserAsync(userMeta);
                }
            }

            return user;
        }

        public async Task<User> CreateUserAsync(UserEntityMetadata meta)
        {
            var user = new User() { Name = meta.displayName, Email = meta.Email(), MsId = meta.id};
            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Get all users in a list
        /// </summary>
        /// <returns></returns>
        public async Task<User[]> GetAllUsersAsync()
        {
            return await this._context.Users.ToArrayAsync();
        }

        /// <summary>
        /// Get user by give email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await this._context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<User> GetUserByIdAsync(int UserId)
        {
            return await this._context.Users
                .FirstOrDefaultAsync(u => u.Id == UserId);
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
            user.CountryId = country.Id;
            
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
            user.OrganisationId = organisation.Id;
            
            await this._context.SaveChangesAsync();
            return user;
        }
    }
}
