using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Commands
{
    public class CreateGroupCommand : IRequest<UpdatedGroupCommandDto>
    {
        public CreateGroupDto Command { get; }

        public CreateGroupCommand(CreateGroupDto command)
        {
            Command = command;
        }
    }

    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, UpdatedGroupCommandDto>
    {
        private readonly IPermissionCheckerService _IPermissionCheckerService;
        private readonly UserRepository _userRepository;
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public CreateGroupCommandHandler(IPermissionCheckerService IPermissionCheckerService, UserRepository userRepository, GroupRepository groupRepository, IAuthService authService)
        {
            _IPermissionCheckerService = IPermissionCheckerService;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _authService = authService;
        }

        public async Task<UpdatedGroupCommandDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUserById(_authService.GetVerifiedCurrentUser().Id, cancellationToken);
            var group = new Group
                (
                    request.Command.Name,
                    request.Command.OrganisationId,
                    currentUser.Id
                );
            bool isAllowed = await _IPermissionCheckerService.CheckPermission(group, currentUser, Operation.Create, cancellationToken);
            if (!isAllowed)
                throw new System.UnauthorizedAccessException($"User does not have permission to create group in organisation");

                await _groupRepository.SaveGroupAsync(group, cancellationToken);

            var adminGroupUser = new GroupUser(group.Id, request.Command.FirstAdminId, Role.Admin);
            await _groupRepository.SaveGroupUserAsync(adminGroupUser, cancellationToken);

            return new UpdatedGroupCommandDto(group);
        }
    }
}