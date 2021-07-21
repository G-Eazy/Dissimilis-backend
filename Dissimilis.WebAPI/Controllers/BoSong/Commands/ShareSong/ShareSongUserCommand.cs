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
    public class ShareSongUserCommand : IRequest<ShortUserDto[]>
    {
        public int SongId { get; }

        public int UserId { get; }

        public ShareSongUserCommand(int songId, int userId)
        {
            SongId = songId;
            UserId = userId;
        }
    }

    public class ShareSongUserCommandHandler : IRequestHandler<ShareSongUserCommand, ShortUserDto[]>
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

        public async Task<ShortUserDto[]> Handle(ShareSongUserCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if (song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            var userToAdd = await _userRepository.GetUserById(request.UserId, cancellationToken);
            var isShared = await _songRepository.GetSongSharedUser(song.Id, userToAdd.Id);

            if (isShared != null || request.UserId == currentUser.Id)
            {
                throw new Exception("User already added to song");
            }

            await _songRepository.CreateAndAddSongShareUser(song, userToAdd);
            try
            {
                await _songRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return song.SharedUsers.Select(s => new ShortUserDto(s.User)).ToArray();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }
        }
    }
}