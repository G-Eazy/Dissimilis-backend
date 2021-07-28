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
        private readonly IPermissionCheckerService _permissionChecker;
        private readonly IAuthService _authService;

        public QueryUserAdminStatusesHandler(IPermissionCheckerService permissionChecker, IAuthService authService)
        {
            _permissionChecker = permissionChecker;
            _authService = authService;
        }

        public async Task<UserAdminStatusDto> Handle(QueryUserAdminStatuses request, CancellationToken cancellationToken)
        {
            var user = _authService.GetVerifiedCurrentUser();
            var adminStatuses = await _permissionChecker.CheckUserAdminStatus(user, cancellationToken);

            return adminStatuses;
        }
    }
}
