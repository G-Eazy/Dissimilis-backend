using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoGroup;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QueryGetGroups : IRequest<GroupIndexDto[]>
    {
        public GetGroupsQueryDto Command { get; }

        public QueryGetGroups(GetGroupsQueryDto command)
        {
            Command = command;
        }
    }

    public class QueryGetGroupsHandler : IRequestHandler<QueryGetGroups, GroupIndexDto[]>
    {
        private readonly IAuthService _authService;
        private readonly GroupRepository _repository;

        public QueryGetGroupsHandler(GroupRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<GroupIndexDto[]> Handle(QueryGetGroups request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var result = await _repository.GetGroups(request.Command.OnlyMyGroups, request.Command.MyAdminGroups,
                request.Command.OrganisationId, currentUser, cancellationToken);

            return result.Select(group => new GroupIndexDto(group)).ToArray();
        }
    }
}