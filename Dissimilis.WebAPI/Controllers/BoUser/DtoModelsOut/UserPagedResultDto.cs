using System;
using System.Collections.Generic;
using System.Linq;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.PagedExtensions;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn;

namespace Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut
{
    public class UserPagedResultDto : PagedResult<UserDto>
    {
        public UsersInMyGroupsDto QueryDto { get; set; }

        public UserPagedResultDto(PagedResult<User> dbResult, UsersInMyGroupsDto queryDto) : base(dbResult)
        {
            QueryDto = queryDto;
            Results = dbResult.Results.Select(u => new UserDto(u)).ToList();
        }

    }
}
