using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using static Dissimilis.Core.Collections.EnumExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
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

        public UpdateSysAdminStatusCommandHandler(UserRepository userRepository, IAuthService authService, PermissionCheckerService IPermissionCheckerService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<UserUpdatedDto> Handle(UpdateSysAdminStatusCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            if (!currentUser.IsSystemAdmin)
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

