using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Commands
{
    public class AddMemberCommand : IRequest<MemberAddedDto>
    {
        public int GroupId { get; }
        public AddMemberDto Command { get; }

        public AddMemberCommand(int groupId, AddMemberDto command)
        {
            GroupId = groupId;
            Command = command;
        }
    }

    public class AddMemberCommandHandler : IRequestHandler<AddMemberCommand, MemberAddedDto>
    {
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _permissionChecker;

        public AddMemberCommandHandler(GroupRepository groupRepository, IAuthService authService, PermissionCheckerService permissionChecker)
        {
            _groupRepository = groupRepository;
            _authService = authService;
            _permissionChecker = permissionChecker;
        }

        public async Task<MemberAddedDto> Handle(AddMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var group = await _groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);

            bool isAllowed = await _permissionChecker.CheckPermission(group, currentUser, Operation.Invite, cancellationToken);
            if (!isAllowed)
                throw new UnauthorizedAccessException("Only an admin can remove other members from the group.");

            var existingGroupUser = await _groupRepository.GetGroupUserAsync(request.Command.NewMemberUserId, request.GroupId, cancellationToken);
            if (existingGroupUser != null) throw new ValidationException("The user is already a member of the group.");

            var newGroupUser = await _groupRepository.AddUserToGroupAsync(request.Command.NewMemberUserId, request.GroupId, request.Command.NewMemberRole, cancellationToken);
            await _groupRepository.UpdateAsync(cancellationToken);

            return new MemberAddedDto() { UserId = newGroupUser.UserId, GroupId = newGroupUser.GroupId };
        }
    }
}
