using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Commands
{
    public class UpdateGroupCommand : IRequest<UpdatedGroupCommandDto>
    {
        public int GroupId { get; set; }
        public UpdateGroupAndOrganisationDto Command { get; }

        public UpdateGroupCommand(int groupId, UpdateGroupAndOrganisationDto command)
        {
            GroupId = groupId;
            Command = command;
        }
    }

    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, UpdatedGroupCommandDto>
    {
        private readonly UserRepository _userRepository;
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public UpdateGroupCommandHandler(UserRepository userRepository, GroupRepository groupRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _authService = authService;
        }

        public async Task<UpdatedGroupCommandDto> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var group = await _groupRepository.GetGroupById(request.GroupId, cancellationToken);
            var hasPermission = await _groupRepository.CheckPermission(group, currentUser, "modify", cancellationToken);

            if (!hasPermission)
                throw new System.UnauthorizedAccessException($"User does not have permission to Update Group");

            group.Name = request.Command.Name;
            group.Address = request.Command.Address;
            group.EmailAddress = request.Command.EmailAddress;
            group.Description = request.Command.Description;
            group.PhoneNumber = request.Command.PhoneNumber;

            await _groupRepository.UpdateAsync(cancellationToken);

            return new UpdatedGroupCommandDto(group);
        }
    }
}
