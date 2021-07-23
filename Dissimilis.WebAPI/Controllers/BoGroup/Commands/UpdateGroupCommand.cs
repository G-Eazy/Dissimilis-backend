using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
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
        private readonly _IPermissionCheckerService _permissionChecker;
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public UpdateGroupCommandHandler(_IPermissionCheckerService permissionChecker, GroupRepository groupRepository, IAuthService authService)
        {
            _permissionChecker = permissionChecker;
            _groupRepository = groupRepository;
            _authService = authService;
        }

        public async Task<UpdatedGroupCommandDto> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var group = await _groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
            var isAllowed = await _permissionChecker.CheckPermission(group, currentUser, Operation.Modify, cancellationToken);

            if (!isAllowed)
                throw new System.UnauthorizedAccessException($"User does not have permission to Update Group");

            group.Name = request.Command?.Name ?? group.Name;
            group.Address = request.Command?.Address ?? group.Address;
            group.Email = request.Command?.Email ?? group.Email;
            group.Description = request.Command?.Description ?? group.Description;
            group.PhoneNumber = request.Command?.PhoneNumber ?? group.PhoneNumber;

            await _groupRepository.UpdateAsync(cancellationToken);

            return new UpdatedGroupCommandDto(group);
        }
    }
}
