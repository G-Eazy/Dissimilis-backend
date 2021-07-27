using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using static Dissimilis.Core.Collections.EnumExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
{
    public class AddUserOrganisationCommand : IRequest<UserOrganisationAddedDto>
    {
        public int OrganisationId { get; }
        public AddUserOrganisationDto Command { get; }

        public AddUserOrganisationCommand(int organisationId, AddUserOrganisationDto command)
        {
            OrganisationId = organisationId;
            Command = command;
        }
    }

    public class AddMemberCommandHandler : IRequestHandler<AddUserOrganisationCommand, UserOrganisationAddedDto>
    {
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public AddMemberCommandHandler(OrganisationRepository organisationRepository, IAuthService authService, PermissionCheckerService IPermissionCheckerService)
        {
            _organisationRepository = organisationRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UserOrganisationAddedDto> Handle(AddUserOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var organisation = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);

            bool isAllowed = await _IPermissionCheckerService.CheckPermission(organisation, currentUser, Operation.Invite, cancellationToken);
            if (!isAllowed)
                throw new UnauthorizedAccessException("Only an admin can add members to the group.");

            var existingOrganisationUser = await _organisationRepository.GetOrganisationUserAsync(organisation.Id, request.Command.NewUserId, cancellationToken);
            if (existingOrganisationUser != null) throw new ValidationException("The user is already a member of the group.");

            var newMemberRoleEnumValue = GetEnumValueFromDescriptionString<Role>(request.Command.NewUserRole);

            var newOrganisationUser = await _organisationRepository.AddUserToOrganisationAsync(request.OrganisationId, request.Command.NewUserId, newMemberRoleEnumValue, cancellationToken);
            await _organisationRepository.UpdateAsync(cancellationToken);

            return new UserOrganisationAddedDto(newOrganisationUser.UserId, newOrganisationUser.OrganisationId, newOrganisationUser.Role.GetDescription());
        }
    }
}
