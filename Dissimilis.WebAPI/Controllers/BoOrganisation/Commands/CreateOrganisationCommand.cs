﻿using System.Threading;
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
    public class CreateOrganisationCommand : IRequest<UpdatedOrganisationCommandDto>
    {
        public CreateOrganisationDto Command { get; }

        public CreateOrganisationCommand(CreateOrganisationDto command)
        {
            Command = command;
        }
    }

    public class CreateOrganisationCommandHandler : IRequestHandler<CreateOrganisationCommand, UpdatedOrganisationCommandDto>
    {
        private readonly UserRepository _userRepository;
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _authService;

        public CreateOrganisationCommandHandler(UserRepository userRepository, OrganisationRepository organisationRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _organisationRepository = organisationRepository;
            _authService = authService;
        }

        public async Task<UpdatedOrganisationCommandDto> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var organisation = new Organisation
                (
                    request.Command.Name,
                    request.Command.Address,
                    request.Command.EmailAddress,
                    request.Command.Description,
                    currentUser.Id
                );
            await _organisationRepository.SaveOrganisationAsync(organisation, cancellationToken);

            var adminUser = await _userRepository.GetUserById(request.Command.FirstAdminId, cancellationToken);
            var adminOrgUser = new OrganisationUser(organisation, adminUser, Role.Admin);
            organisation.Users.Add(adminOrgUser);
            await _organisationRepository.UpdateAsync(cancellationToken);

            return new UpdatedOrganisationCommandDto(organisation);
        }
    }
}
