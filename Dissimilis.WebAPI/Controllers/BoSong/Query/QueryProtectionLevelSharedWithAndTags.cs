using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Query
{
    public class QueryProtectionLevelSharedWithAndTags : IRequest<ProtectionLevelSharedWithAndTagsDto>
    {
        public int SongId { get; }

        public QueryProtectionLevelSharedWithAndTags(int songId)
        {
            SongId = songId;
        }
    }

    public class QueryProtectionLevelSharedWithAndTagsHandler : IRequestHandler<QueryProtectionLevelSharedWithAndTags, ProtectionLevelSharedWithAndTagsDto>
    {
        private readonly SongRepository _repository;

        public QueryProtectionLevelSharedWithAndTagsHandler(SongRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProtectionLevelSharedWithAndTagsDto> Handle(QueryProtectionLevelSharedWithAndTags request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFullSongById(request.SongId, cancellationToken);

            return new ProtectionLevelSharedWithAndTagsDto(result);
        }
    }
}