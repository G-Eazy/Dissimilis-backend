using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class DuplicateSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public DuplicateSongDto Command { get; }

        public DuplicateSongCommand(int songId, DuplicateSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class DuplicateSongCommandHandler : IRequestHandler<DuplicateSongCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public DuplicateSongCommandHandler(SongRepository songRepository, IAuthService authService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _IPermissionCheckerService = IPermissionCheckerService;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(DuplicateSongCommand request, CancellationToken cancellationToken)
        {
            var duplicateFromSong = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _authService.GetVerifiedCurrentUser();

            if (!await _IPermissionCheckerService.CheckPermission(duplicateFromSong, currentUser, Operation.Get, cancellationToken)) throw new UnauthorizedAccessException();

            var duplicatedSong = duplicateFromSong.CloneWithUpdatedArrangerId(currentUser.Id, request.Command.Title);
            duplicatedSong.SetUpdated(currentUser.Id);
            await _songRepository.SaveAsync(duplicatedSong, cancellationToken);

            return new UpdatedSongCommandDto(duplicatedSong);
        }
    }
}