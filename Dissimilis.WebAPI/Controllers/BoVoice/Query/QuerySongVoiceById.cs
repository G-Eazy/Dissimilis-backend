using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Query
{
    public class QuerySongVoiceById : IRequest<SongVoiceDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }

        public QuerySongVoiceById(int songId, int songVoiceId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
        }
    }

    public class QuerySongVoiceByIdHandler : IRequestHandler<QuerySongVoiceById, SongVoiceDto>
    {
        private readonly VoiceRepository _repository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public QuerySongVoiceByIdHandler(VoiceRepository repository, SongRepository songRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _repository = repository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<SongVoiceDto> Handle(QuerySongVoiceById request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Get, cancellationToken)) throw new UnauthorizedAccessException();
            
            var result = await _repository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            

            return new SongVoiceDto(result);
        }
    }
}