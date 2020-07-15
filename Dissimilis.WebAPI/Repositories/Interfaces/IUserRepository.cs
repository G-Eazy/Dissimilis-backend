using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using Experis.Ciber.Web.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Reposities.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByMsIdAsync(Guid id);
        Task<UserDTO> GetUserDTOByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User[]> GetAllUsersAsync();
        Task<User> CreateUserAsync(UserEntityMetadata user);
        Task<User> CreateOrFindUserAsync(UserEntityMetadata user);
        Task<User> UpdateUserOrganisationAsync(User user, Organisation organisation);
        Task<User> UpdateUserCountryAsync(User user, Country country);
    }
}
