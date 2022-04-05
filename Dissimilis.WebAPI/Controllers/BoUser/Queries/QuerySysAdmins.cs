using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QuerySysAdmins : IRequest<UserDto[]> { }

    public class QuerySysAdminsHandler : IRequestHandler<QuerySysAdmins, UserDto[]>
    {
        private readonly UserRepository _repository;
        private readonly IAuthService _authService;
        
        public QuerySysAdminsHandler(UserRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UserDto[]> Handle(QuerySysAdmins request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            if (!currentUser.IsSystemAdmin)
                throw new UnauthorizedAccessException($"User {currentUser.Name} does not have the privileges to delete this user");

            var users = await _repository.GetAllUsers(cancellationToken);

            return users
                .Where(u => u.IsSystemAdmin)
                .Select(u => new UserDto(u))
                .ToArray();
        }
    }
}
