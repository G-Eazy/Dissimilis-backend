using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
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

        public RemoveShareSongUserCommandHandler(SongRepository songRepository, UserRepository userRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _IAuthService = IAuthService;
        }

        public async Task<ShortUserDto[]> Handle(RemoveShareSongUserCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if (song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            var userToAdd = await _userRepository.GetUserById(request.UserId, cancellationToken);
            var SharedSongUser = await _songRepository.GetSongSharedUser(song.Id, userToAdd.Id);

            if (SharedSongUser == null || request.UserId == currentUser.Id)
            {
                throw new Exception($"Song witd id: {request.SongId} is not shared with user with id: {request.UserId}");
            }
            
            userToAdd.SongsShared.Remove(SharedSongUser);
            song.SharedUsers.Remove(SharedSongUser);
            await _songRepository.DeleteSongSharedUser(song, userToAdd, SharedSongUser, cancellationToken);
            return song.SharedUsers.Select(s => new ShortUserDto(s.User)).ToArray();
        }
    }
}