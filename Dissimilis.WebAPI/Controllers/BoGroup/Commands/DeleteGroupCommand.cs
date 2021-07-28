using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Commands
{
    public class DeleteGroupCommand : IRequest<UpdatedGroupCommandDto>
    {
        public int GroupId { get; set; }

        public DeleteGroupCommand(int groupId)
        {
            GroupId = groupId;
        }
    }

    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, UpdatedGroupCommandDto>
    {
        private readonly IPermissionCheckerService _IPermissionCheckerService;
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public DeleteGroupCommandHandler(IPermissionCheckerService IPermissionCheckerService, GroupRepository groupRepository, IAuthService authService)
        {
            _IPermissionCheckerService = IPermissionCheckerService;
            _groupRepository = groupRepository;
            _authService = authService;
        }

        public async Task<UpdatedGroupCommandDto> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var group = await _groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);

            if (group == null)
                throw new NotFoundException($"Group with Id {request.GroupId} not found");

            bool isAllowed = await _IPermissionCheckerService.CheckPermission(group, currentUser, Operation.Delete, cancellationToken);
            if (!isAllowed)
                throw new System.UnauthorizedAccessException($"User does not have permission to delete group in organisation");

            await _groupRepository.DeleteGroupAsync(group, cancellationToken);

            return null;
        }
    }
}