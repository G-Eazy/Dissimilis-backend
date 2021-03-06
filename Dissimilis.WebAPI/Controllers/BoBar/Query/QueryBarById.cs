using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoBar.Query
{
    public class QueryBarById : IRequest<BarDto>
    {
        public int SongId { get; }
        public int VoiceId { get; }
        public int BarId { get; }

        public QueryBarById(int songId, int voiceId, int barId)
        {
            SongId = songId;
            VoiceId = voiceId;
            BarId = barId;
        }
    }

    public class QuerySongByIdHandler : IRequestHandler<QueryBarById, BarDto>
    {
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;


        public QuerySongByIdHandler(BarRepository barRepository, SongRepository songRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _barRepository = barRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<BarDto> Handle(QueryBarById request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Get, cancellationToken)) throw new UnauthorizedAccessException();

            var result = await _barRepository.GetSongBarById(request.SongId, request.VoiceId, request.BarId, cancellationToken);

            return new BarDto(result);
        }
    }
}