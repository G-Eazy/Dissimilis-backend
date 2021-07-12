using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
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

        public QuerySongByIdHandler(SongRepository repository)
        {
            _repository = repository;
        }

        public async Task<SongByIdDto> Handle(QuerySongById request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFullSongById(request.SongId, cancellationToken);

            return new SongByIdDto(result);
        }
    }
}