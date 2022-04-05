using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class CreateTransposedSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public TransposeSongDto Command { get; }

        public CreateTransposedSongCommand(int songId, TransposeSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class CreateTransposedSongCommandHandler : IRequestHandler<CreateTransposedSongCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public CreateTransposedSongCommandHandler(SongRepository songRepository, IAuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedSongCommandDto> Handle(CreateTransposedSongCommand request, CancellationToken cancellationToken)
        {

            var duplicateFromSong = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _authService.GetVerifiedCurrentUser();

            if (!await _IPermissionCheckerService.CheckPermission(duplicateFromSong, currentUser, Operation.Get, cancellationToken)) throw new UnauthorizedAccessException();

            Song duplicatedSong = null;

            using (var transaction = _songRepository.Context.Database.BeginTransaction())
            {
                try
                {
                    duplicatedSong = duplicateFromSong.CloneWithUpdatedArrangerId(currentUser.Id, request.Command.Title);

                    duplicatedSong = duplicatedSong.Transpose(request.Command.Transpose);
                    duplicatedSong.SetUpdated(currentUser);
                }
                catch
                {
                    transaction.Rollback();
                    await _songRepository.UpdateAsync(cancellationToken);
                }
            }
            await _songRepository.SaveAsync(duplicatedSong, cancellationToken);

            return new UpdatedSongCommandDto(duplicatedSong);
        }
    }
}
