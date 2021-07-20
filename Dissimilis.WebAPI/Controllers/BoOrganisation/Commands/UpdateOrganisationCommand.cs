using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
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
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _authService;

        public UpdateOrganisationCommandHandler(OrganisationRepository organisationRepository, IAuthService authService)
        {
            _organisationRepository = organisationRepository;
            _authService = authService;
        }

        public async Task<UpdatedOrganisationCommandDto> Handle(UpdateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var organisation = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);
            var hasPermission = await _organisationRepository.CheckPermission(organisation, currentUser, "modify", cancellationToken);

            if (!hasPermission)
                throw new System.UnauthorizedAccessException($"User does not have permission to Update organisation");

            organisation.Name = request.Command?.Name ?? organisation.Name;
            organisation.Address = request.Command?.Address ?? organisation.Address;
            organisation.EmailAddress = request.Command?.EmailAddress ?? organisation.EmailAddress;
            organisation.Description = request.Command?.Description ?? organisation.Description;
            organisation.PhoneNumber = request.Command?.PhoneNumber ?? organisation.PhoneNumber;

            await _organisationRepository.UpdateAsync(cancellationToken);

            return new UpdatedOrganisationCommandDto(organisation);
        }
    }
}
