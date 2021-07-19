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
        public ChangeUserRoleDto Command { get; set; }

        public ChangeUserRoleCommand(int groupId, ChangeUserRoleDto command)
        {
            GroupId = groupId;
            Command = command;
        }
    }

    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, UserRoleChangedDto>
    {
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public ChangeUserRoleCommandHandler(GroupRepository groupRepository, IAuthService authService)
        {
            _groupRepository = groupRepository;
            _authService = authService;
        }
        public async Task<UserRoleChangedDto> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            bool isUserAdmin = await _groupRepository.CheckUserAdminAsync(currentUser.Id, request.GroupId, cancellationToken);
            if (!isUserAdmin) throw new UnauthorizedAccessException("Only an admin can change another user's role.");

            await using var transaction = await _groupRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var updatedGroupUser = await _groupRepository.ChangeUserRoleAsync(request.Command.MemberId, request.GroupId, request.Command.RoleToSet, cancellationToken);

            return new UserRoleChangedDto() { UserId = updatedGroupUser.UserId, GroupId = updatedGroupUser.GroupId, UpdatedRole = updatedGroupUser.Role };
        }
    }
}
