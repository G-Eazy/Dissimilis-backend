using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Commands
{
    public class UpdateSysAdminStatusCommand : IRequest<UserUpdatedDto>
    {
        public int UserId { get; }
        public UpdateSysAdminStatusDto Command { get; }

        public UpdateSysAdminStatusCommand(int organisationId, UpdateSysAdminStatusDto command)
        {
            UserId = organisationId;
            Command = command;
        }
    }

    public class UpdateSysAdminStatusCommandHandler : IRequestHandler<UpdateSysAdminStatusCommand, UserUpdatedDto>
    {
        private readonly UserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _permissionCheckerService;

        public UpdateSysAdminStatusCommandHandler(UserRepository userRepository, IAuthService authService, IPermissionCheckerService permissionCheckerService)
        {
            _userRepository = userRepository;
            _authService = authService;
            _permissionCheckerService = permissionCheckerService;
        }

        public async Task<UserUpdatedDto> Handle(UpdateSysAdminStatusCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            if (!_permissionCheckerService.IsAdminUser(currentUser))
                throw new UnauthorizedAccessException($"User {currentUser.Name} does not have the privileges to edit sysadmin status");

            var changeSysAdminStatusUser = await _userRepository.GetUserById(request.UserId, cancellationToken);
            if (changeSysAdminStatusUser == null)
                throw new NotFoundException($"User with id {request.UserId} was not found");

            changeSysAdminStatusUser.IsSystemAdmin = request.Command.IsSystemAdmin;
            await _userRepository.UpdateAsync(cancellationToken);

            return new UserUpdatedDto(changeSysAdminStatusUser.Id);
        }
    }
}

