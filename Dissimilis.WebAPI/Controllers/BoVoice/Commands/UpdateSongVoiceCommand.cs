using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class UpdateSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        public CreateSongVoiceDto Command { get; }

        public UpdateSongVoiceCommand(int songId, int songVoiceId, CreateSongVoiceDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class UpdateSongVoiceCommandHandler : IRequestHandler<UpdateSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly VoiceRepository _voiceRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public UpdateSongVoiceCommandHandler(VoiceRepository voiceRepository, SongRepository songRepository, IAuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _voiceRepository = voiceRepository;
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }
            song.PerformSnapshot(currentUser);

            songVoice.VoiceNumber = request.Command?.VoiceNumber ?? songVoice.VoiceNumber;
            songVoice.VoiceName = request.Command?.VoiceName ?? songVoice.VoiceName;
            songVoice.SetSongVoiceUpdated(_authService.GetVerifiedCurrentUser().Id);
            
            await _voiceRepository.UpdateAsync(cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}