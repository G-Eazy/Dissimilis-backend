using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
{
    public class UpdateOrganisationCommand : IRequest<UpdatedOrganisationCommandDto>
    {
        public int OrganisationId { get; set; }
        public UpdateGroupAndOrganisationDto Command { get; }

        public UpdateOrganisationCommand(int organisationId, UpdateGroupAndOrganisationDto command)
        {
            OrganisationId = organisationId;
            Command = command;
        }
    }

    public class UpdateOrganisationCommandHandler : IRequestHandler<UpdateOrganisationCommand, UpdatedOrganisationCommandDto>
    {
        private readonly IPermissionCheckerService _IPermissionCheckerService;
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _authService;

        public UpdateOrganisationCommandHandler(IPermissionCheckerService IPermissionCheckerService, OrganisationRepository organisationRepository, IAuthService authService)
        {
            _IPermissionCheckerService = IPermissionCheckerService;
            _organisationRepository = organisationRepository;
            _authService = authService;
        }

        public async Task<UpdatedOrganisationCommandDto> Handle(UpdateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var organisation = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);
            var isAllowed = await _IPermissionCheckerService.CheckPermission(organisation, currentUser, Operation.Modify, cancellationToken);

            if (!isAllowed)
                throw new System.UnauthorizedAccessException($"User does not have permission to Update organisation");

            organisation.Name = request.Command?.Name ?? organisation.Name;
            organisation.Address = request.Command?.Address ?? organisation.Address;
            organisation.Email = request.Command?.Email ?? organisation.Email;
            organisation.Description = request.Command?.Description ?? organisation.Description;
            organisation.PhoneNumber = request.Command?.PhoneNumber ?? organisation.PhoneNumber;

            await _organisationRepository.UpdateAsync(cancellationToken);

            return new UpdatedOrganisationCommandDto(organisation);
        }
    }
}
