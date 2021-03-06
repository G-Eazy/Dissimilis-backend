using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Query
{
    public class QueryOrganisationById : IRequest<OrganisationByIdDto>
    {
        public int OrganisationId { get; }

        public QueryOrganisationById(int organisationId)
        {
            OrganisationId = organisationId;
        }
    }

    public class QueryOrganisationByIdHandler : IRequestHandler<QueryOrganisationById, OrganisationByIdDto>
    {
        private readonly IPermissionCheckerService _IPermissionCheckerService;
        private readonly IAuthService _authService;
        private readonly OrganisationRepository _organisationRepository;

        public QueryOrganisationByIdHandler(IPermissionCheckerService IPermissionCheckerService, OrganisationRepository repository, IAuthService authService)
        {
            _IPermissionCheckerService = IPermissionCheckerService;
            _authService = authService;
            _organisationRepository = repository;
        }

        public async Task<OrganisationByIdDto> Handle(QueryOrganisationById request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var org = await _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, cancellationToken);
            
            if(org == null)
                throw new NotFoundException($"Organisation with Id {request.OrganisationId} not found");

            bool allowed = await _IPermissionCheckerService.CheckPermission(org, currentUser, Operation.Get, cancellationToken);
            if (!allowed)
                throw new ForbiddenException($"User with id {currentUser.Id} is not allowed to view this organisation");

            return new OrganisationByIdDto(org);
        }
    }
}
