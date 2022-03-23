using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Exceptions;

namespace Dissimilis.WebAPI.Controllers.BoUser.Commands
{
    public class DeleteUserCommand : IRequest<UserUpdatedDto>
    {
        public int UserId { get; }

        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserUpdatedDto>
    {
        private readonly UserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _permissionChecker;

        public DeleteUserCommandHandler(UserRepository userRepository, IAuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _userRepository = userRepository;
            _authService = authService;
            _permissionChecker = IPermissionCheckerService;
        }

        public async Task<UserUpdatedDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var userToBeDeleted = await _userRepository.GetUserById(request.UserId, cancellationToken);
            if (userToBeDeleted == null)
                throw new NotFoundException($"User with id {request.UserId} not found");

            if (!_permissionChecker.IsAdminUser(currentUser))
                throw new UnauthorizedAccessException($"User {currentUser.Name} does not have the privileges to delete this user");

            await _userRepository.DeleteUser(userToBeDeleted, cancellationToken);

            return null;
        }
    }
}

