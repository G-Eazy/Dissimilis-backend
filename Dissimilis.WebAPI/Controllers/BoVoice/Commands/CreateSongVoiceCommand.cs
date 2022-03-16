using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class CreateSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public CreateSongVoiceDto Command { get; }

        public CreateSongVoiceCommand(int songId, CreateSongVoiceDto command)
        {
            SongId = songId;
            Command = command;

        }
    }

    public class CreateSongVoiceCommandHandler : IRequestHandler<CreateSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly VoiceRepository _voiceRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;


        public CreateSongVoiceCommandHandler(VoiceRepository voiceRepository, SongRepository songRepository, IAuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _voiceRepository = voiceRepository;
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }

        public async Task<UpdatedCommandDto> Handle(CreateSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            if (song.Voices.Any(v => v.VoiceNumber == request.Command.VoiceNumber))
            {
                throw new ValidationException("Voice number already used");
            }
            if (string.IsNullOrEmpty(request.Command.VoiceName))
            {
                throw new ValidationException("VoiceName not defined");
            }
            song.PerformSnapshot(currentUser);

            var nextVoiceNumber = song.Voices.OrderByDescending(v => v.VoiceNumber).FirstOrDefault()?.VoiceNumber ?? 0;
            nextVoiceNumber++;
            var songVoice = new SongVoice()
            {
                VoiceNumber = request.Command.VoiceNumber ?? nextVoiceNumber,
                Instrument = null,
                Song = song,
                VoiceName = request.Command.VoiceName
            };

            var cloneVoice = song.Voices.FirstOrDefault();
            song.Voices.Add(songVoice);

            song.SyncBarCountToMaxInAllVoices();
            song.SetUpdatedOverAll(currentUser.Id);

            if (cloneVoice != null)
            {
                song.SyncVoicesFrom(cloneVoice);
            }
            await _voiceRepository.UpdateAsync(cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}