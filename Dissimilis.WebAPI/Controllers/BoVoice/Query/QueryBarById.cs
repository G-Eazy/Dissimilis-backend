using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
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
        private readonly Repository _repository;

        public QuerySongByIdHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<BarDto> Handle(QueryBarById request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetSongBarById(request.SongId, request.PartId, request.BarId, cancellationToken);

            return new BarDto(result);
        }
    }
}