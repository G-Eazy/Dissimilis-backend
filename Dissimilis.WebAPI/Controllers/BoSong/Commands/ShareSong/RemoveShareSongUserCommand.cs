using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class RemoveShareSongUserCommand : IRequest<ShortUserDto[]>
    {
        public int SongId { get; }

        public int UserId { get; }

        public RemoveShareSongUserCommand(int songId, int userId)
        {
            SongId = songId;
            UserId = userId;
        }
    }

    public class RemoveShareSongUserCommandHandler : IRequestHandler<RemoveShareSongUserCommand, ShortUserDto[]>
    {
        private readonly SongRepository _songRepository;
        private readonly UserRepository _userRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public RemoveShareSongUserCommandHandler(SongRepository songRepository, UserRepository userRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }

        public async Task<ShortUserDto[]> Handle(RemoveShareSongUserCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongWithTagsSharedUsers(request.SongId, cancellationToken);
            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            var userToAdd = await _userRepository.GetUserById(request.UserId, cancellationToken);
            var SharedSongUser = await _songRepository.GetSongSharedUser(song.Id, userToAdd.Id);

            if (SharedSongUser == null || request.UserId == currentUser.Id)
            {
                throw new Exception($"Song witd id: {request.SongId} is not shared with user with id: {request.UserId}");
            }
            await _songRepository.DeleteSongSharedUser(SharedSongUser, cancellationToken);
            return song.SharedUsers.Select(s => new ShortUserDto(s.User)).ToArray();
        }
    }
}