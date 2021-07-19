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
    public class SetMemberAdminCommand : IRequest<MemberRoleChangedDto>
    {
        public int GroupId { get; set; }
        public ChangeMemberRoleDto Command { get; set; }

        public SetMemberAdminCommand(int groupId, ChangeMemberRoleDto command)
        {
            GroupId = groupId;
            Command = command;
        }
    }

    public class SetMemberAdminCommandHandler : IRequestHandler<SetMemberAdminCommand, MemberRoleChangedDto>
    {
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public SetMemberAdminCommandHandler(GroupRepository groupRepository, IAuthService authService)
        {
            _groupRepository = groupRepository;
            _authService = authService;
        }
        public async Task<MemberRoleChangedDto> Handle(SetMemberAdminCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            bool isUserAdmin = await _groupRepository.CheckUserAdminAsync(currentUser.Id, request.GroupId, cancellationToken);
            if (!isUserAdmin) throw new UnauthorizedAccessException("Only an admin can set another member's role to admin.");

            await using var transaction = await _groupRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var updatedGroupUser = await _groupRepository.SetUserAdminAsync(request.Command.MemberId, request.GroupId, cancellationToken);

            return new MemberRoleChangedDto() { UserId = updatedGroupUser.UserId, GroupId = updatedGroupUser.GroupId, UpdatedRole = updatedGroupUser.Role };
        }
    }
}
