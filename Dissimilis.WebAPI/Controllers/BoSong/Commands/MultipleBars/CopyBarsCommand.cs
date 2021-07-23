using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars
{
    public class CopyBarsCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public CopyBarDto Command { get; }


        public CopyBarsCommand(int songId, CopyBarDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class CopyBarsCommandHandler : IRequestHandler<CopyBarsCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public CopyBarsCommandHandler(SongRepository songRepository, IAuthService IAuthService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedSongCommandDto> Handle(CopyBarsCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            song.CopyBars(request.Command.FromPosition, request.Command.CopyLength, request.Command.ToPosition);

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);

            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}