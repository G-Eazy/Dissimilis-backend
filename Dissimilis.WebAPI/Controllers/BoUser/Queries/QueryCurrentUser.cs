using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryCurrentUser : IRequest<UserDto> { }

    public class QueryCurrentUserHandler : IRequestHandler<QueryCurrentUser, UserDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public QueryCurrentUserHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UserDto> Handle(QueryCurrentUser request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            if (currentUser == null)
                return null;

            var result = await _repository.GetUserById(currentUser.Id, cancellationToken);

            return new UserDto(result);
        }
    }
}
