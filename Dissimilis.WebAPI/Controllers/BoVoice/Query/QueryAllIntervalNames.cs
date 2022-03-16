using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Query
{
    public class QueryAllIntervalNames : IRequest<IntervalNamesDto>
    {
        public int SongId { get; set; }
        public int SongVoiceId { get; set; }

        public QueryAllIntervalNames(int songId, int songVoiceId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
        }

        public class QueryAllIntervalNamesHandler : IRequestHandler<QueryAllIntervalNames, IntervalNamesDto>
        {
            private readonly SongRepository _songRepository;
            private readonly VoiceRepository _voiceRepository;
            private readonly IAuthService _authService;
            private readonly IPermissionCheckerService _permissionCheckerService;

            public QueryAllIntervalNamesHandler(SongRepository songRepository, VoiceRepository voiceRepository, IAuthService authService, IPermissionCheckerService permissionCheckerService)
            {
                _songRepository = songRepository;
                _voiceRepository = voiceRepository;
                _authService = authService;
                _permissionCheckerService = permissionCheckerService;
            }
            public async Task<IntervalNamesDto> Handle(QueryAllIntervalNames request, CancellationToken cancellationToken)
            {
                var currentUser = _authService.GetVerifiedCurrentUser();
                var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);

                if (!await _permissionCheckerService.CheckPermission(song, currentUser, Operation.Get, cancellationToken))
                    throw new UnauthorizedAccessException($"User with id {currentUser.Id} does not have read access to song with id {song.Id}.");

                var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);

                return new IntervalNamesDto() { IntervalNames = songVoice.GetAllIntervalNames().SortIntervalNames() };
            }
        }
    }
}
