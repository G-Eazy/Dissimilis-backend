using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Commands
{
    public class RemoveMemberCommand : IRequest<MemberRemovedDto>
    {
        public int GroupId { get; }
        public int UserId { get; }

        public RemoveMemberCommand(int groupId, int userId)
        {
            GroupId = groupId;
            UserId = userId;
        }
    }

    public class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, MemberRemovedDto>
    {
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _permissionChecker;

        public RemoveMemberCommandHandler(GroupRepository groupRepository, IAuthService authService, PermissionCheckerService permissionChecker)
        {
            _groupRepository = groupRepository;
            _authService = authService;
            _permissionChecker = permissionChecker;
        }

        public async Task<MemberRemovedDto> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var group = await _groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);

            bool isAllowed = await _permissionChecker.CheckPermission(group, currentUser, Operation.Kick, cancellationToken);
            if (!isAllowed)
                throw new UnauthorizedAccessException("Only an admin can remove other members from the group.");

            var groupUserToDelete = await _groupRepository.GetGroupUserAsync(request.UserId, request.GroupId, cancellationToken);
            if (groupUserToDelete == null)
                throw new ValidationException("The user is not a member of the group.");

            if (await _groupRepository.IsUserLastAdmin(groupUserToDelete.UserId, groupUserToDelete.GroupId, cancellationToken))
                throw new InvalidOperationException("The member cannot be removed from the group as it is the last admin.");

            var deletedGroupUser = await _groupRepository.RemoveUserFromGroupAsync(request.UserId, request.GroupId, cancellationToken);
            await _groupRepository.UpdateAsync(cancellationToken);

            return new MemberRemovedDto() { UserId = deletedGroupUser.UserId, GroupId = deletedGroupUser.GroupId };
        }
    }
}
