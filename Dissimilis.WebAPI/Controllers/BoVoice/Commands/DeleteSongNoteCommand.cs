using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class DeleteSongNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; }
        public int SongNoteId { get; }

        public DeleteSongNoteCommand(int songId, int songVoiceId, int songBarId, int songNoteId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            SongNoteId = songNoteId;
        }
    }

    public class DeleteSongNoteCommandHandler : IRequestHandler<DeleteSongNoteCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;

        public DeleteSongNoteCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongNoteCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

            var note = part.Notes.FirstOrDefault(n => n.Id == request.SongNoteId);
            if (note == null)
            {
                throw new NotFoundException($"Note with Id {request.SongNoteId} not found");
            }

            part.Notes.Remove(note);

            await _repository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}