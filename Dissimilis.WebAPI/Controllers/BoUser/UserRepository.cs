using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.Core.Collections;
using Dissimilis.Core.Types;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.PagedExtensions;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoUser
{
    public class UserRepository
    {

        private DissimilisDbContext _context;
        public UserRepository(DissimilisDbContext context)
        {
            _context = context;
        }

        public async Task<User[]> GetAllUsers(CancellationToken cancellationToken)
        {
            return await _context.Users
                .OrderBy(u => u.Name)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<PagedResult<User>> GetPagedUsersInMyGroups(UsersInMyGroupsDto user, User currentUser, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .OrderBy(u => u.Email)
                .AsQueryable();

            if (!currentUser.IsSystemAdmin)
            {
                var myGroupIds = await _context.GroupUsers
                    .Where(gu => gu.UserId == currentUser.Id)
                    .Select(gu => gu.GroupId)
                    .ToArrayAsync(cancellationToken);

                query = query.Where(u => u.Groups.Any(g => myGroupIds.Contains(g.GroupId)));
            }

            if (user.SearchText.NotNullOrWhiteSpace())
            {
                var searchText = $"%{user.SearchText.Trim()}%";
                query = query.Where(u => EF.Functions.Like(u.Name, searchText) ||
                                         EF.Functions.Like(u.Email, searchText));
            }

            if (user.OrganizationFilter.Any())
            {
                query = query.Where(u => u.Organisations.Any(o => user.OrganizationFilter.Contains(o.OrganisationId)));
            }

            if (user.GroupFilter.Any())
            {
                query = query.Where(u => u.Groups.Any(o => user.GroupFilter.Contains(o.GroupId)));
            }


            var result = await query
                .GetPaged(user, cancellationToken);

            return result;
        }

        public async Task<User> GetUserById(int userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException($"User with Id {userId} not found");
            }
            return user;
        }

        public async Task<User> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new NotFoundException($"User with email {email} not found");
            return user;
        }

        public async Task<User> CreateOrFindUserAsync(UserEntityMetadata userMeta)
        {
            var user = await GetUserByMsIdAsync(userMeta.id);
            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    user = AddEmailToUser(user, userMeta);
                    user.Name = userMeta.displayName;
                    await UpdateAsync(null);
                }
                return user;
            }

            user = await GetUserByEmailAsync(userMeta.Email());
            if (user != null)
            {
                user.MsId = userMeta.id;
                user.Name = userMeta.displayName;
                await UpdateAsync(null);
                return user;
            }

            user = await CreateUserAsync(userMeta);

            return user;
        }

        public User AddEmailToUser(User user, UserEntityMetadata meta)
        {
            user.Email = (meta.Email() != string.Empty ? meta.Email() : (meta.mail ?? meta.userPrincipalName)).ToLower();
            return user;
        }

        public async Task<User> CreateUserAsync(UserEntityMetadata meta)
        {
            var user = new User()
            {
                Name = meta.displayName,
                Email = meta.Email() ?? (meta.mail ?? meta.userPrincipalName).ToLower(),
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


        public async Task DeleteUser(User user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            await UpdateAsync(cancellationToken);
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
        public async Task UpdateAsync(CancellationToken? cancellationToken = null)
        {
            if (cancellationToken.HasValue)
            {
                await _context.SaveChangesAsync(cancellationToken.Value);
            }
            else
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<int>> GetOrganisationUserIds(User user)
        {
            return await _context.OrganisationUsers
                .Where(ou => ou.UserId == user.Id)
                .Select(ou => ou.OrganisationId)
                .ToListAsync();
        }

        public async Task<List<int>> GetGroupUserIds(User user)
        {
            return await _context.GroupUsers
                .Where(gu => gu.UserId == user.Id)
                .Select(gu => gu.GroupId)
                .ToListAsync();
        }
    }
}
