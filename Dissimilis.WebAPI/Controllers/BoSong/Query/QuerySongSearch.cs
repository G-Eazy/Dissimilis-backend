using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class QuerySongSearch : IRequest<SongByIdDto[]>
    {
        public SearchQueryDto Command { get; }


        public QuerySongSearch(SearchQueryDto command)
        {
            Command = command;
        }
    }

    public class QuerySongSearchHandler : IRequestHandler<QuerySongSearch, SongByIdDto[]>
    {
        private readonly Repository _repository;

        public QuerySongSearchHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<SongByIdDto[]> Handle(QuerySongSearch request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetSongSearchList(request.Command, cancellationToken);

            return result.Select(s => new SongByIdDto(s)).ToArray();
        }
    }
}