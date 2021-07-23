using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Commands
{
    public class ChangeUserRoleCommand : IRequest<UserRoleChangedDto>
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public ChangeUserRoleDto Command { get; set; }

        public ChangeUserRoleCommand(int groupId, int userId, ChangeUserRoleDto command)
        {
            GroupId = groupId;
            UserId = userId;
            Command = command;
        }
    }

    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, UserRoleChangedDto>
    {
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _permissionCheckerService;

        public ChangeUserRoleCommandHandler(GroupRepository groupRepository, IAuthService authService, IPermissionCheckerService permissionCheckerService)
        {
            _groupRepository = groupRepository;
            _authService = authService;
            _permissionCheckerService = permissionCheckerService;
        }
        public async Task<UserRoleChangedDto> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var group = await _groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);

            bool isAllowed = await _permissionCheckerService.CheckPermission(group, currentUser, Operation.Modify, cancellationToken);
            if (!isAllowed) throw new UnauthorizedAccessException("Only an admin can change another user's role.");

            var updatedGroupUser = await _groupRepository.ChangeUserRoleAsync(request.UserId, request.GroupId, request.Command.RoleToSet, cancellationToken);

            return new UserRoleChangedDto() { UserId = updatedGroupUser.UserId, GroupId = updatedGroupUser.GroupId, UpdatedRole = updatedGroupUser.Role };
        }
    }
}
