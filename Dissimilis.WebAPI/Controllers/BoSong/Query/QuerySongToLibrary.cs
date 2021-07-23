using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QuerySongToLibrary : IRequest<SongIndexDto[]> 
    { 
        public bool GetDeleted { get; }

        public QuerySongToLibrary(bool getDeleted = false)
        {
            GetDeleted = getDeleted;
        }
    }

    public class QuerySongToLibraryHandler : IRequestHandler<QuerySongToLibrary, SongIndexDto[]>
    {
        private readonly SongRepository _repository;
        private readonly IAuthService _authService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;


        public QuerySongToLibraryHandler(SongRepository repository, IAuthService authService, _IPermissionCheckerService IPermissionCheckerService) 
        {
            _repository = repository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }

        public async Task<SongIndexDto[]> Handle(QuerySongToLibrary request, CancellationToken cancellationToken)
        {
            var user = _authService.GetVerifiedCurrentUser();

            Song[] result = null;

            if (!request.GetDeleted)
                result = await _repository.GetAllSongsInMyLibrary(user.Id, cancellationToken);
            else
                result = await _repository.GetMyDeletedSongs(user, cancellationToken);

            foreach(var song in result)
            {
                if (!await _IPermissionCheckerService.CheckPermission(song, user, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();
            }

            return result.Select(s => new SongIndexDto(s)).ToArray();
        }
    }
}
