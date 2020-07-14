using Dissimilis.WebAPI.Controllers.BoUser.DTOs;
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

        private DissimilisDbContext _context;
        public UserRepository(DissimilisDbContext _context)
        {
            this._context = _context;
        }

        public async Task<UserDTO[]> AllUsersQuery()
        {
            var UserModelArray = await this._context.Users.ToArrayAsync();
            var UserDTOArray = UserModelArray.Select(u => new UserDTO(u)).ToArray();
            return UserDTOArray;
        }

        public async Task<UserDTO> UserByIdQuery(int UserId)
        {
            var UserModel = await this._context.Users
                .FirstOrDefaultAsync(u => u.Id == UserId);

            UserDTO UserObject = null;
            if (UserModel != null) {
                UserObject = new UserDTO(UserModel);
            }
            return UserObject;

        }
        
    }
}
