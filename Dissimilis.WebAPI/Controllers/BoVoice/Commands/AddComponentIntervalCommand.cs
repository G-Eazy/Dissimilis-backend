using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class AddComponentIntervalCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public AddComponentIntervalDto Command { get; }

        public AddComponentIntervalCommand(int songId, int songVoiceId, AddComponentIntervalDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class AddComponentIntervalHandler : IRequestHandler<AddComponentIntervalCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly VoiceRepository _voiceRepository;
        private readonly AuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;


        public AddComponentIntervalHandler(VoiceRepository voiceRepository, SongRepository songRepository, AuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _voiceRepository = voiceRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }
        public async Task<UpdatedCommandDto> Handle(AddComponentIntervalCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();


            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);

            song.PerformSnapshot(currentUser);

            var sourceVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.Command.SourceVoiceId, cancellationToken);

            songVoice.AddComponentInterval(sourceVoice, request.Command.IntervalPosition);
            songVoice.SetSongVoiceUpdated(_authService.GetVerifiedCurrentUser().Id);

            await _voiceRepository.UpdateAsync(cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}
