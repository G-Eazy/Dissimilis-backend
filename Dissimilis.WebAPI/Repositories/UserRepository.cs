using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.Reposities.Interfaces;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Experis.Ciber.Web.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Reposities;
using Dissimilis.WebAPI.DTOs;

namespace Dissimilis.WebAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private DissimilisDbContext context;
        public UserRepository(DissimilisDbContext context)
        {
            this.context = context;
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
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();

            //add the user to the lowest usergroup
            var userGroup = await this.context.UserGroups.SingleOrDefaultAsync(ug => ug.Name == "User");
            await this.context.UserGroupMembers.AddAsync(new UserGroupMembers() { UserId = user.Id, UserGroupId = userGroup.Id });
            
            await this.context.SaveChangesAsync();
            Console.WriteLine(user + " " + userGroup);
            return user;
        }

        /// <summary>
        /// Get all users in a list
        /// </summary>
        /// <returns></returns>
        public async Task<User[]> GetAllUsersAsync()
        {
            return await this.context.Users.ToArrayAsync();
        }

        /// <summary>
        /// Get user by give email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await this.context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<User> GetUserByIdAsync(int UserId)
        {
            return await this.context.Users
                .FirstOrDefaultAsync(u => u.Id == UserId);
        }

        public async Task<UserDTO> GetUserDTOByIdAsync(int id)
        {
            User user = await GetUserByIdAsync(id);
            UserDTO userModel = new UserDTO(user);

            return userModel;
        }

        /// <summary>
        /// Get user by the microsoft ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetUserByMsIdAsync(string id)
        {
            return await context.Users.SingleOrDefaultAsync(x => x.MsId == id);
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
            
            await this.context.SaveChangesAsync();
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
            
            await this.context.SaveChangesAsync();
            return user;
        }
    }
}
