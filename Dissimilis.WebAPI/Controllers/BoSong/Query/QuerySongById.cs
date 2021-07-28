using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QuerySongById : IRequest<SongByIdDto>
    {
        public int SongId { get; }

        public QuerySongById(int songId)
        {
            SongId = songId;
        }
    }

    public class QuerySongByIdHandler : IRequestHandler<QuerySongById, SongByIdDto>
    {
        private readonly SongRepository _repository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public QuerySongByIdHandler(SongRepository repository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<SongByIdDto> Handle(QuerySongById request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            if (!await _IPermissionCheckerService.CheckPermission(result, currentUser, Operation.Get, cancellationToken)) throw new UnauthorizedAccessException();

            bool currentUserHasWriteAccess = await _IPermissionCheckerService.CheckPermission(result, currentUser, Operation.Modify, cancellationToken);

            return new SongByIdDto(result, currentUserHasWriteAccess);
        }
    }
}