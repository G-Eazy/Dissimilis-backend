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

        public AddMemberCommandHandler(GroupRepository groupRepository, IAuthService authService)
        {
            _groupRepository = groupRepository;
            _authService = authService;
        }

        public async Task<MemberAddedDto> Handle(AddMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            bool isUserAdmin = await _groupRepository.CheckUserAdminAsync(currentUser.Id, request.GroupId, cancellationToken);
            if (!isUserAdmin) throw new UnauthorizedAccessException("Only an admin can add new members to the group.");

            var existingGroupUser = await _groupRepository.GetGroupUserAsync(request.Command.NewMemberUserId, request.GroupId, cancellationToken);
            if (existingGroupUser != null) throw new ValidationException("The user is already a member of the group.");

            var newGroupUser = await _groupRepository.AddUserToGroupAsync(request.Command.NewMemberUserId, request.GroupId, request.Command.NewMemberRole, cancellationToken);
            await _groupRepository.UpdateAsync(cancellationToken);

            return new MemberAddedDto() { UserId = newGroupUser.UserId, GroupId = newGroupUser.GroupId };
        }
    }
}
