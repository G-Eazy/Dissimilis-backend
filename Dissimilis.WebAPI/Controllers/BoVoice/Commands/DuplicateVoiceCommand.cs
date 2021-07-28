using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class DuplicateVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }

        public CreateSongVoiceDto Command { get; set; }

        public DuplicateVoiceCommand(int songId, int songVoiceId, CreateSongVoiceDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class DuplicateVoiceCommandHandler : IRequestHandler<DuplicateVoiceCommand, UpdatedCommandDto>
    {
        private readonly VoiceRepository _voiceRepository;
        private readonly SongRepository _songRepository;
        private readonly AuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public DuplicateVoiceCommandHandler(VoiceRepository voiceRepository, SongRepository songRepository, AuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _voiceRepository = voiceRepository;
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedCommandDto> Handle(DuplicateVoiceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }
            if (string.IsNullOrEmpty(request.Command.VoiceName))
            {
                throw new Exception("Voicename can't be a empty string");
            }
            song.PerformSnapshot(currentUser);

            var duplicatedVoice = songVoice.Clone(request.Command.VoiceName, currentUser, null, song.Voices.Max(v => v.VoiceNumber));
            song.Voices.Add(duplicatedVoice);

            await _voiceRepository.UpdateAsync(cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(duplicatedVoice);
        }
    }
}