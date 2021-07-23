using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QuerySongSearch : IRequest<SongIndexDto[]>
    {
        public SearchQueryDto Command { get; }


        public QuerySongSearch(SearchQueryDto command)
        {
            Command = command;
        }
    }

    public class QuerySongSearchHandler : IRequestHandler<QuerySongSearch, SongIndexDto[]>
    {
        private readonly SongRepository _repository;
        private readonly IAuthService _authService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public QuerySongSearchHandler(SongRepository repository, IAuthService authService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _repository = repository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<SongIndexDto[]> Handle(QuerySongSearch request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();

            var result = await _repository.GetSongSearchList(currentUser, request.Command, cancellationToken);
            foreach (var song in result)
            {
                if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Get, cancellationToken))
                {
                    result.Remove(song);
                }
            }
            return result.Select(s => new SongIndexDto(s)).ToArray();
        }
    }
}