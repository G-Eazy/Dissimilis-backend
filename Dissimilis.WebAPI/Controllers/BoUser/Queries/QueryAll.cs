using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryAll : IRequest<UserDto[]> { }

    public class QueryAllHandler : IRequestHandler<QueryAll, UserDto[]>
    {
        private readonly UserRepository _repository;
        private readonly AuthService _authService;

        public QueryAllHandler(UserRepository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UserDto[]> Handle(QueryAll request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            if (!currentUser.IsSystemAdmin)
                throw new UnauthorizedAccessException($"User {currentUser.Name} does not have the privileges to delete this user");

            var result = await _repository.GetAllUsers(cancellationToken);

            return result
                .Select(u => new UserDto(u))
                .ToArray();
        }
    }
}