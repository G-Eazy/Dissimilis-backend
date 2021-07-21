using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QuerySharedWithAndTags : IRequest<SharedWithAndTagsDto>
    {
        public int SongId { get; }

        public QuerySharedWithAndTags(int songId)
        {
            SongId = songId;
        }
    }

    public class QuerySharedWithAndTagsHandler : IRequestHandler<QuerySharedWithAndTags, SharedWithAndTagsDto>
    {
        private readonly SongRepository _repository;

        public QuerySharedWithAndTagsHandler(SongRepository repository)
        {
            _repository = repository;
        }

        public async Task<SharedWithAndTagsDto> Handle(QuerySharedWithAndTags request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFullSongById(request.SongId, cancellationToken);

            return new SharedWithAndTagsDto(result);
        }
    }
}