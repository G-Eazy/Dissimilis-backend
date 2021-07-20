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
        private readonly UserRepository _userRepository;
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public CreateGroupCommandHandler(UserRepository userRepository, GroupRepository groupRepository, IAuthService authService)
        {
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
                    request.Command.Address,
                    request.Command.PhoneNumber,
                    request.Command.Email,
                    request.Command.Description,
                    currentUser.Id
                );
            if (!await _groupRepository.CheckPermission(request.Command.OrganisationId, currentUser, "add", cancellationToken))
                throw new System.UnauthorizedAccessException($"User does not have permission to create group in organisation");

                await _groupRepository.SaveGroupAsync(group, cancellationToken);

            var adminUser = await _userRepository.GetUserById(request.Command.FirstAdminId, cancellationToken);
            var adminGroupUser = new GroupUser(group.Id, adminUser.Id, Role.Admin);
            group.Users.Add(adminGroupUser);
            currentUser.Groups.Add(adminGroupUser);
            await _groupRepository.UpdateAsync(cancellationToken);
            await _userRepository.UpdateAsync(cancellationToken);

            return new UpdatedGroupCommandDto(group);
        }
    }
}