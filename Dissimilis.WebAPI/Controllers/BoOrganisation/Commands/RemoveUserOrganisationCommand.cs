using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
{
    public class RemoveUserOrganisationCommand : IRequest<UserOrganisationUpdatedDto>
    {
        public int OrganisationId { get; }
        public int UserId { get; }

        public RemoveUserOrganisationCommand(int organisationId, int userId)
        {
            OrganisationId = organisationId;
            UserId = userId;
        }
    }

    public class RemoveUserOrganisationCommandHandler : IRequestHandler<RemoveUserOrganisationCommand, UserOrganisationUpdatedDto>
    {
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public RemoveUserOrganisationCommandHandler(OrganisationRepository organisationRepository, IAuthService authService, PermissionCheckerService IPermissionCheckerService)
        {
            _organisationRepository = organisationRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UserOrganisationUpdatedDto> Handle(RemoveUserOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var organisation = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);

            bool isAllowed = 
                await _IPermissionCheckerService.CheckPermission(organisation, currentUser, Operation.Kick, cancellationToken)
                || request.UserId == currentUser.Id;
            if (!isAllowed)
                throw new UnauthorizedAccessException("Only an admin can remove other members from the organisation.");

            var organisationUserToDelete = await _organisationRepository.GetOrganisationUserAsync(request.OrganisationId, request.UserId, cancellationToken);
            if (organisationUserToDelete == null)
                throw new ValidationException("The user requested for removal is not a member of the organisation.");

            if (await _organisationRepository.IsUserLastAdmin(organisationUserToDelete.OrganisationId, organisationUserToDelete.UserId, cancellationToken))
                throw new InvalidOperationException("The member cannot be removed from the organisation as it is the last admin.");

            var deletedOrganisationUser = await _organisationRepository.RemoveUserFromOrganisationAsync(request.OrganisationId, request.UserId, cancellationToken);
            await _organisationRepository.UpdateAsync(cancellationToken);

            return new UserOrganisationUpdatedDto()
            { 
                UserId = deletedOrganisationUser.UserId,
                OrganisationId = deletedOrganisationUser.OrganisationId,
                Role = deletedOrganisationUser.Role.GetDescription()
            };
        }
    }
}
