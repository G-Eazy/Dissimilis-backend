using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class QuerySongUndoStackById : IRequest<SongUndoStackByIdDto>
    {
        public int SongId { get; }  

        public QuerySongUndoStackById(int songId)
        {
            SongId = songId;
        }

        public class QuerySongUndoStackByIdHandler : IRequestHandler<QuerySongUndoStackById, SongUndoStackByIdDto>
        {
            private readonly SongRepository _repository;

            public QuerySongUndoStackByIdHandler(SongRepository repository)
            {
                _repository = repository;
            }

            public async Task<SongUndoStackByIdDto> Handle(QuerySongUndoStackById request, CancellationToken cancellationToken)
            {
                var result = await _repository.GetFullSongById(request.SongId, cancellationToken);

                return new SongUndoStackByIdDto(result);
            }
        }
    }
}
