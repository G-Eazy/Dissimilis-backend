using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoBar.Query
{
    public class QueryBarById : IRequest<BarDto>
    {
        public int SongId { get; }
        public int PartId { get; }
        public int BarId { get; }

        public QueryBarById(int songId, int partId, int barId)
        {
            SongId = songId;
            PartId = partId;
            BarId = barId;
        }
    }

    public class QuerySongByIdHandler : IRequestHandler<QueryBarById, BarDto>
    {
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;


        public QuerySongByIdHandler(BarRepository barRepository, SongRepository songRepository, IAuthService IAuthService, _IPermissionCheckerService IPermissionCheckerService)
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

            var result = await _barRepository.GetSongBarById(request.SongId, request.PartId, request.BarId, cancellationToken);

            return new BarDto(result);
        }
    }
}