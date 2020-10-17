using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
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
        private readonly AuthService _authService;

        public DeleteSongNoteCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
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

            part.SongVoice.SetSongVoiceUpdated(_authService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}