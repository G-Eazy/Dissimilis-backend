using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands.ShareSong
{
    public class ShareSongUserCommand : IRequest<ShortUserDto[]>
    {
        public int SongId { get; }

        public string UserEmail { get; }

        public ShareSongUserCommand(int songId, string userEmail)
        {
            SongId = songId;
            UserEmail = userEmail;
        }
    }

    public class ShareSongUserCommandHandler : IRequestHandler<ShareSongUserCommand, ShortUserDto[]>
    {
        private readonly SongRepository _songRepository;
        private readonly UserRepository _userRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public ShareSongUserCommandHandler(SongRepository songRepository, UserRepository userRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<ShortUserDto[]> Handle(ShareSongUserCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongWithTagsSharedUsers(request.SongId, cancellationToken);
            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            var userToAdd = await _userRepository.GetUserByEmail(request.UserEmail, cancellationToken);
            var isShared = await _songRepository.GetSongSharedUser(song.Id, userToAdd.Id);

            if (isShared != null || userToAdd.Id == currentUser.Id)
            {
                throw new Exception("User already added to song");
            }

            await _songRepository.CreateAndAddSongShareUser(song, userToAdd);
            return song.SharedUsers.Select(s => new ShortUserDto(s.User)).ToArray();
        }
    }
}