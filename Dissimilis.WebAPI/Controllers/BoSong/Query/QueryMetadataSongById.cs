using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QueryMetadataSongById : IRequest<SongMetadataDto>
    {
        public int SongId { get; }

        public QueryMetadataSongById(int songId)
        {
            SongId = songId;
        }
    }

    public class QueryMetadataSongByIdHandler : IRequestHandler<QueryMetadataSongById, SongMetadataDto>
    {
        private readonly SongRepository _repository;
        private readonly IAuthService _IAuthService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public QueryMetadataSongByIdHandler(SongRepository repository, IAuthService IAuthService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<SongMetadataDto> Handle(QueryMetadataSongById request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            if (!await _IPermissionCheckerService.CheckPermission(result, currentUser, Operation.Get, cancellationToken)) throw new UnauthorizedAccessException();

            return new SongMetadataDto(result);
        }
    }
}