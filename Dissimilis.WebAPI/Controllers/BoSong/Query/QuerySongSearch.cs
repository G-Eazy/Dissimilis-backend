using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
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
        private readonly IPermissionCheckerService _permissionCheckerService;

        public QuerySongSearchHandler(SongRepository repository, IAuthService authService, IPermissionCheckerService permissionCheckerService)
        {
            _repository = repository;
            _authService = authService;
            _permissionCheckerService = permissionCheckerService;
        }

        public async Task<SongIndexDto[]> Handle(QuerySongSearch request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();

            var result = await _repository.GetSongSearchList(currentUser, request.Command, cancellationToken);
            var resultWithPermissionField = new List<SongIndexDto>();

            foreach (var song in result)
            {
                var currentUserHasWriteAccess = await _permissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken);
                resultWithPermissionField.Add(new SongIndexDto(song, currentUserHasWriteAccess));
            }

            return resultWithPermissionField.ToArray();
        }
    }
}