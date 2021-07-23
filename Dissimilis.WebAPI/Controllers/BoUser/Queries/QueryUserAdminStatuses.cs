using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryUserAdminStatuses : IRequest<UserAdminStatusDto>{ }

    public class QueryUserAdminStatusesHandler : IRequestHandler<QueryUserAdminStatuses, UserAdminStatusDto>
    {
        private readonly UserRepository _repository;
        private readonly IAuthService _authService;

        public QueryUserAdminStatusesHandler(UserRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UserAdminStatusDto> Handle(QueryUserAdminStatuses request, CancellationToken cancellationToken)
        {
            var user = _authService.GetVerifiedCurrentUser();
            var adminStatuses = await _repository.GetUserAdminStatuses(user);

            return adminStatuses;
        }
    }
}
