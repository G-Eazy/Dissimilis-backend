using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoUser
{
    public class UserRepository
    {
        private DissimilisDbContext context;
        public UserRepository(DissimilisDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Fetch all users in the database
        /// </summary>
        /// <returns>200</returns>
        public async Task<UserDTO[]> AllUsersQuery()
        {
            var UserModelArray = await this.context.Users.ToArrayAsync();
            var UserDTOArray = UserModelArray.Select(u => new UserDTO(u)).ToArray();
            return UserDTOArray;
        }

        public async Task<UserDTO> UserByIdQuery(int UserId)
        {
            var UserModel = await this.context.Users
                .SingleOrDefaultAsync(u => u.Id == UserId);

            UserDTO UserObject = null;
            if (UserModel != null) {
                UserObject = new UserDTO(UserModel);
            }
            return UserObject;

        }
        
    }
}
