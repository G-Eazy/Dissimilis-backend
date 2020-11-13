using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{

    // Should probably pipeline this using IPipelineBehaviour
    public class QuerySongToLibrary : IRequest<SongIndexDto[]>
    {
        public QuerySongToLibrary() { }
    }

    public class QuerySongToLibraryHandler : IRequestHandler<QuerySongToLibrary, SongIndexDto[]>
    {
        private readonly Repository _repository;
        private readonly IAuthService _authService;

        public QuerySongToLibraryHandler(Repository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<SongIndexDto[]> Handle(QuerySongToLibrary request, CancellationToken cancellationToken)
        {
            var userId = _authService.GetCurrentUserId();

            if (!userId.HasValue)
                return null;

            var result = await _repository.GetAllSongs(userId.Value, cancellationToken);

            return result.Select(s => new SongIndexDto(s)).ToArray();
        }
    }
}
