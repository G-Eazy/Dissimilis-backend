using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoNote.Query
{
    public class QuerySongNoteById : IRequest<NoteDto>
    {
        public int SongNoteId { get; }

        public QuerySongNoteById(int songNoteId)
        {
            SongNoteId = songNoteId;
        }
    }

    public class QuerySongNoteByIdHandler : IRequestHandler<QuerySongNoteById, NoteDto>
    {
        private readonly NoteRepository _noteRepository;

        public QuerySongNoteByIdHandler(NoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<NoteDto> Handle(QuerySongNoteById request, CancellationToken cancellationToken)
        {
            var result = await _noteRepository.GetSongNoteById(request.SongNoteId, cancellationToken);

            return new NoteDto(result);
        }
    }
}