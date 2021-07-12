using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
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

        public QuerySongSearchHandler(SongRepository repository)
        {
            _repository = repository;
        }

        public async Task<SongIndexDto[]> Handle(QuerySongSearch request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetSongSearchList(request.Command, cancellationToken);

            return result.Select(s => new SongIndexDto(s)).ToArray();
        }
    }
}