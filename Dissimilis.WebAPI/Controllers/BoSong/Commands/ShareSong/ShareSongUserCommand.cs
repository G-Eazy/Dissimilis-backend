using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class ShareSongUserCommand : INotification
    {
        public int SongId { get; }
        public ShareSongDto Command { get; }


        public ShareSongUserCommand(int songId, ShareSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class ShareSongUserCommandHandler : INotificationHandler<ShareSongUserCommand>
    {
        private readonly SongRepository _songRepository;
        private readonly UserRepository _userRepository;
        private readonly IAuthService _IAuthService;

        public ShareSongUserCommandHandler(SongRepository songRepository, UserRepository userRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _IAuthService = IAuthService;
        }

        public async Task Handle(ShareSongUserCommand notification, CancellationToken cancellationToken)
        {
            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(notification.SongId, cancellationToken);
            if(song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            foreach( var user in notification.Command.ShareSongIds)
            {
                var userToAdd = await _userRepository.GetUserById(user, cancellationToken);
                var isShared = await _songRepository.GetSongSharedUser(song.Id, userToAdd.Id);
                
                if(isShared != null || user == currentUser.Id)
                {
                    throw new Exception("User already added to song");
                }
                
                var songSharedUser = new SongSharedUser()
                {
                    UserId = userToAdd.Id,
                    SongId = song.Id
                };
                userToAdd.SongsShared.Add(songSharedUser);
                song.SharedUsers.Add(songSharedUser);
            }
            try
            {
                await _songRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }


        }
    }
}