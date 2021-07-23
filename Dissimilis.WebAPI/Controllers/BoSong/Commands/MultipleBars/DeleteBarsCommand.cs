using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars
{
    public class DeleteBarsCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public DeleteBarDto Command { get; }

        public DeleteBarsCommand(int songId, DeleteBarDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class DeleteBarsCommandHandler : IRequestHandler<DeleteBarsCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;


        public DeleteBarsCommandHandler(SongRepository songRepository, IAuthService IAuthService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }

        public async Task<UpdatedSongCommandDto> Handle(DeleteBarsCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            song.DeleteBars(request.Command.FromPosition, request.Command.DeleteLength);

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);

            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}
