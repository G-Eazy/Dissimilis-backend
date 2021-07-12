using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QuerySongToLibrary : IRequest<SongIndexDto[]> { }

    public class QuerySongToLibraryHandler : IRequestHandler<QuerySongToLibrary, SongIndexDto[]>
    {
        private readonly SongRepository _repository;
        private readonly IAuthService _authService;

        public QuerySongToLibraryHandler(SongRepository repository, IAuthService authService) 
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<SongIndexDto[]> Handle(QuerySongToLibrary request, CancellationToken cancellationToken)
        {
            var user = _authService.GetVerifiedCurrentUser();

            if (user == null)
                return null;

            var result = await _repository.GetAllSongsInMyLibrary(user.Id, cancellationToken);

            return result.Select(s => new SongIndexDto(s)).ToArray();
        }
    }
}
