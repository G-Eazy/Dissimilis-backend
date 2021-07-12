using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QueryMetadataSongById : IRequest<SongMetadataDto>
    {
        public int SongId { get; }

        public QueryMetadataSongById(int songId)
        {
            SongId = songId;
        }
    }

    public class QueryMetadataSongByIdHandler : IRequestHandler<QueryMetadataSongById, SongMetadataDto>
    {
        private readonly SongRepository _repository;

        public QueryMetadataSongByIdHandler(SongRepository repository)
        {
            _repository = repository;
        }

        public async Task<SongMetadataDto> Handle(QueryMetadataSongById request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFullSongById(request.SongId, cancellationToken);

            return new SongMetadataDto(result);
        }
    }
}