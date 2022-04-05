using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
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
    public class DuplicateAllChordsCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public DuplicateAllChordsDto Command { get; }

        public DuplicateAllChordsCommand(int songId, int songVoiceId, DuplicateAllChordsDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class DuplicateAllChordsHandler : IRequestHandler<DuplicateAllChordsCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly VoiceRepository _voiceRepository;
        private readonly AuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public DuplicateAllChordsHandler(VoiceRepository voiceRepository, SongRepository songRepository, AuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _voiceRepository = voiceRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }
        public async Task<UpdatedCommandDto> Handle(DuplicateAllChordsCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }
            var sourceVoice = songVoice.Song.Voices.SingleOrDefault(v => v.Id == request.Command.SourceVoiceId);
            if (sourceVoice == null)
            {
                throw new NotFoundException($"Source voice with id {request.Command.SourceVoiceId} not found");
            }

            song.PerformSnapshot(currentUser);

            songVoice.DuplicateAllChords(sourceVoice, request.Command.IncludeComponentIntervals);
            songVoice.SetSongVoiceUpdated(currentUser.Id);
            await _voiceRepository.UpdateAsync(cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}
