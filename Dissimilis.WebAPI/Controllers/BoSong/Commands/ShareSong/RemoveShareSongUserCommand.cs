using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
    public class RemoveShareSongUserCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }

        public int UserId { get; }

        public RemoveShareSongUserCommand(int songId, int userId)
        {
            SongId = songId;
            UserId = userId;
        }
    }

    public class RemoveShareSongUserCommandHandler : IRequestHandler<RemoveShareSongUserCommand, UpdatedSongCommandDto>
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

        public async Task<UpdatedSongCommandDto> Handle(RemoveShareSongUserCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
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
            await _songRepository.DeleteSongSharedUser(SharedSongUser, cancellationToken);
            try
            {
                await _songRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new UpdatedSongCommandDto(song);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }
        }
    }
}