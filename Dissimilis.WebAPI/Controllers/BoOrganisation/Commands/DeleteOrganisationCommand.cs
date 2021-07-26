using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
{
    public class DeleteOrganisationCommand : IRequest<UpdatedOrganisationCommandDto>
    {
        public int OrganisationId { get; set; }

        public DeleteOrganisationCommand(int organisationId)
        {
            OrganisationId = organisationId;
        }
    }

    public class DeleteOrganisationCommandHandler : IRequestHandler<DeleteOrganisationCommand, UpdatedOrganisationCommandDto>
    {
        private readonly IPermissionCheckerService _IPermissionCheckerService;
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _authService;

        public DeleteOrganisationCommandHandler(IPermissionCheckerService IPermissionCheckerService, OrganisationRepository organisationRepository, IAuthService authService)
        {
            _IPermissionCheckerService = IPermissionCheckerService;
            _organisationRepository = organisationRepository;
            _authService = authService;
        }

        public async Task<UpdatedOrganisationCommandDto> Handle(DeleteOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var organisation = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);

            if (organisation == null)
                throw new NotFoundException($"Organisation with Id {request.OrganisationId} not found");

            bool isAllowed = await _IPermissionCheckerService.CheckPermission(organisation, currentUser, Operation.Delete, cancellationToken);
            if (!isAllowed)
                throw new System.UnauthorizedAccessException($"User does not have permission to delete organisation");

            await _organisationRepository.DeleteOrganisationAsync(organisation, cancellationToken);

            return null;
        }
    }
}
