using System;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using System.Threading;
using static Dissimilis.Core.Collections.EnumExtensions;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
{
    public class ChangeUserRoleCommand: IRequest<UserRoleChangedDto>
    {

            public int OrganisationId { get; set; }
            public int UserId { get; set; }
            public ChangeUserRoleDto Command { get; set; }

            public ChangeUserRoleCommand(int organisationId, int userId, ChangeUserRoleDto command)
            {
                OrganisationId = organisationId;
                UserId = userId;
                Command = command;
            }
        }

        public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, UserRoleChangedDto>
        {
            private readonly OrganisationRepository _organisationRepository;
            private readonly IAuthService _authService;
            private readonly IPermissionCheckerService _permissionCheckerService;

            public ChangeUserRoleCommandHandler(OrganisationRepository organisationRepository, IAuthService authService, IPermissionCheckerService permissionCheckerService)
            {
                _organisationRepository = organisationRepository;
                _authService = authService;
                _permissionCheckerService = permissionCheckerService;
            }
            public async Task<UserRoleChangedDto> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
            {
                var currentUser = _authService.GetVerifiedCurrentUser();
                var organisation = await _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, cancellationToken);

                bool isAllowed = await _permissionCheckerService.CheckPermission(organisation, currentUser, Operation.Modify, cancellationToken);
                if (!isAllowed) throw new UnauthorizedAccessException("Only an admin can change another user's role.");

                var roleToSetEnumValue = GetEnumValueFromDescriptionString<Role>(request.Command.RoleToSet);

                bool isLastAdmin = await _organisationRepository.IsUserLastAdmin(organisation.Id, request.UserId, cancellationToken);
                if (isLastAdmin && roleToSetEnumValue == Role.Member)
                    throw new InvalidOperationException("The user cannot be set to member as it is the last admin of the organisation.");

                var updatedOrganisationUser = await _organisationRepository.ChangeUserRoleAsync(request.UserId, request.OrganisationId, roleToSetEnumValue, cancellationToken);

                return new UserRoleChangedDto() { UserId = updatedOrganisationUser.UserId, OrganisationId = updatedOrganisationUser.OrganisationId, UpdatedRole = updatedOrganisationUser.Role };
            }
        }
    }
