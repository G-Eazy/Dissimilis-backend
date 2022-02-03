using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryUserById : IRequest<UserDto>
    {
        public int UserId { get; }

        public QueryUserById(int userId)
        {
            UserId = userId;
        }
    }

    public class QueryUserByIdHandler : IRequestHandler<QueryUserById, UserDto>
    {
        private readonly UserRepository _repository;
        private readonly IAuthService _authService;

        public QueryUserByIdHandler(UserRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UserDto> Handle(QueryUserById request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            if (!currentUser.IsSystemAdmin && currentUser.Id != request.UserId)
                throw new UnauthorizedAccessException($"User {currentUser.Name} does not have the privileges to delete this user");

            var user = await _repository.GetUserById(request.UserId, cancellationToken);

            return new UserDto(user);
        }
    }
}