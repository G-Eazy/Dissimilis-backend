using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Query
{
    public class QueryGroupById : IRequest<GroupByIdDto>
    {
        public int GroupId { get; }

        public QueryGroupById(int groupId)
        {
            GroupId = groupId;
        }
    }

    public class QueryGroupByIdHandler : IRequestHandler<QueryGroupById, GroupByIdDto>
    {
        private readonly IPermissionCheckerService _IPermissionCheckerService;
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _IAuthService;

        public QueryGroupByIdHandler(IPermissionCheckerService IPermissionCheckerService, GroupRepository repository, IAuthService authService)
        {
            _IPermissionCheckerService = IPermissionCheckerService;
            _groupRepository = repository;
            _IAuthService = authService;
        }

        public async Task<GroupByIdDto> Handle(QueryGroupById request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var group = await _groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);

            if (group == null)
                throw new NotFoundException($"Group with Id {request.GroupId} not found");

            bool allowed = await _IPermissionCheckerService.CheckPermission(group, currentUser, Operation.Get, cancellationToken);
            if (!allowed)
                throw new ForbiddenException($"User with id {currentUser.Id} is not allowed to view this group");

            return new GroupByIdDto(group);
        }
    }
}
