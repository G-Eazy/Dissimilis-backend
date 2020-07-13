using Dissimilis.WebAPI.Controllers.BoUser.DTOs;
using Dissimilis.WebAPI.Database;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryAllUsers : IRequest<UserDTO[]>
    {
        public QueryAllUsers() { }
    }

    public class QueryAllUsersHandler : IRequestHandler<QueryAllUsers, UserDTO[]>
    {
        private UserRepository _repository;
        public QueryAllUsersHandler(DissimilisDbContext context)
        {
            this._repository = new UserRepository(context);
        }

        public async Task<UserDTO[]> Handle(QueryAllUsers request, CancellationToken cancellationToken)
        {
            var UserModelArray = await _repository.GetAllUsers(cancellationToken);
            var UserDTOArray = UserModelArray.Select(u => new UserDTO(u)).ToArray();
            return UserDTOArray;
        }

        
    }
}
