using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IsolationLevel = System.Data.IsolationLevel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoBar.Commands
{
    public class DeleteSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int BarId { get; }

        public DeleteSongBarCommand(int songId, int songVoiceId, int barId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            BarId = barId;
        }
    }

    public class DeleteSongBarCommandHandler : IRequestHandler<DeleteSongBarCommand, UpdatedCommandDto>
    {
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;


        public DeleteSongBarCommandHandler(BarRepository repository, SongRepository songRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _barRepository = repository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongBarCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken))throw new UnauthorizedAccessException();

            var songVoice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with Id {request.SongVoiceId} was not found");
            }

            var bar = songVoice.SongBars.FirstOrDefault(b => b.Id == request.BarId);
            if (bar == null)
            {
                throw new NotFoundException($"Bar with Id {request.BarId} was not found");
            }
            song.PerformSnapshot(currentUser);

            song.RemoveSongBarFromAllVoices(bar.Position);
            song.SetUpdatedOverAll(currentUser.Id);
            song.SyncVoicesFrom(songVoice);
            await _songRepository.UpdateAsync(cancellationToken);
            await _barRepository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}
