using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Query
{
    public class QuerySongVoiceById : IRequest<SongVoiceDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }

        public QuerySongVoiceById(int songId, int songVoiceId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
        }
    }

    public class QuerySongVoiceByIdHandler : IRequestHandler<QuerySongVoiceById, SongVoiceDto>
    {
        private readonly VoiceRepository _repository;

        public QuerySongVoiceByIdHandler(VoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<SongVoiceDto> Handle(QuerySongVoiceById request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);

            return new SongVoiceDto(result);
        }
    }
}