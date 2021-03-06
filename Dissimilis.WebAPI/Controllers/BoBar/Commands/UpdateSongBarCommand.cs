using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using System;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoVoice;

namespace Dissimilis.WebAPI.Controllers.BoBar.Commands
{
    public class UpdateSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        public int BarId { get; }
        public UpdateBarDto Command { get; }

        public UpdateSongBarCommand(int songId, int songVoiceId, int barId, UpdateBarDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            BarId = barId;
            Command = command;
        }
    }

    public class UpdateSongBarCommandHandler : IRequestHandler<UpdateSongBarCommand, UpdatedCommandDto>
    {
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public UpdateSongBarCommandHandler(BarRepository barRepository, SongRepository songRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _barRepository = barRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongBarCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            var voice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (voice == null)
            {
                throw new NotFoundException($"Voice with Id {request.SongVoiceId} not found");
            }

            var bar = voice.SongBars.FirstOrDefault(b => b.Id == request.BarId);
            if (bar == null)
            {
                throw new NotFoundException($"Bar with Id {request.BarId} not found");
            }
            song.PerformSnapshot(currentUser);

            bar.RepAfter = request.Command?.RepAfter ?? bar.RepAfter;
            bar.RepBefore = request.Command?.RepBefore ?? bar.RepBefore;
            bar.VoltaBracket = request.Command?.VoltaBracket ?? bar.VoltaBracket;
            if (bar.VoltaBracket == 0)
            {
                bar.VoltaBracket = null;
            }

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);
            song.SyncVoicesFrom(voice);
            //await _barRepository.UpdateAsync(cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(bar);
        }
    }
}
