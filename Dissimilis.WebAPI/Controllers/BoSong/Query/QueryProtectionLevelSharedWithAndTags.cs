using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QueryProtectionLevelSharedWithAndTags : IRequest<ProtectionLevelSharedWithAndTagsDto>
    {
        public int SongId { get; }

        public QueryProtectionLevelSharedWithAndTags(int songId)
        {
            SongId = songId;
        }
    }

    public class QueryProtectionLevelSharedWithAndTagsHandler : IRequestHandler<QueryProtectionLevelSharedWithAndTags, ProtectionLevelSharedWithAndTagsDto>
    {
        private readonly SongRepository _repository;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;
        private readonly IAuthService _IAuthService;

        public QueryProtectionLevelSharedWithAndTagsHandler(SongRepository repository, _IPermissionCheckerService IPermissionCheckerService, IAuthService authService)
        {
            _repository = repository;
            _IPermissionCheckerService = IPermissionCheckerService;
            _IAuthService = authService;
        }

        public async Task<ProtectionLevelSharedWithAndTagsDto> Handle(QueryProtectionLevelSharedWithAndTags request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Get, cancellationToken)) throw new UnauthorizedAccessException();

            var result = await _repository.GetSongWithTagsSharedUsers(request.SongId, cancellationToken);

            return new ProtectionLevelSharedWithAndTagsDto(result);
        }
    }
}