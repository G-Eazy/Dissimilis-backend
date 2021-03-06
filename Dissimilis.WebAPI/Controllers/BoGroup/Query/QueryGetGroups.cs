using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Query
{
    public class QueryGetGroups : IRequest<GroupIndexDto[]>
    {
        public string FilterBy { get; }
        public int? OrganisationId { get; }

        public QueryGetGroups(string filterBy, int? organisationId)
        {
            OrganisationId = organisationId;
            FilterBy = filterBy;
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
            var result = await _repository.GetGroupsAsync(request.OrganisationId, request.FilterBy, currentUser, cancellationToken);

            return result.Select(group => new GroupIndexDto(group)).ToArray();
        }
    }
}