using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
{
    public class UpdateOrganisationCommand : IRequest<UpdatedOrganisationCommandDto>
    {
        public UpdateOrganisationDto Command { get; }

        public UpdateOrganisationCommand(UpdateOrganisationDto command)
        {
            Command = command;
        }
    }

    public class UpdateOrganisationCommandHandler : IRequestHandler<UpdateOrganisationCommand, UpdatedOrganisationCommandDto>
    {
        private readonly UserRepository _userRepository;
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _authService;

        public UpdateOrganisationCommandHandler(UserRepository userRepository, OrganisationRepository organisationRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _organisationRepository = organisationRepository;
            _authService = authService;
        }

        public async Task<UpdatedOrganisationCommandDto> Handle(UpdateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var organisation = new Organisation
                (
                    request.Command.Name,
                    request.Command.Address,
                    request.Command.EmailAddress,
                    request.Command.Description,
                    request.Command.PhoneNumber,
                    currentUser.Id
                );
            if (!await _organisationRepository.CheckPermission(organisation, currentUser, "add", cancellationToken))
                throw new System.UnauthorizedAccessException($"User does not have permission to Update organisation");


            await _organisationRepository.SaveOrganisationAsync(organisation, cancellationToken);

            var adminUser = await _userRepository.GetUserById(request.Command.FirstAdminId, cancellationToken);
            var adminOrgUser = new OrganisationUser(organisation.Id, adminUser.Id, Role.Admin);
            organisation.Users.Add(adminOrgUser);
            await _organisationRepository.UpdateAsync(cancellationToken);

            return new UpdatedOrganisationCommandDto(organisation);
        }
    }
}
