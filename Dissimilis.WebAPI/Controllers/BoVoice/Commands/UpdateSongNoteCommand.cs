using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class UpdateSongNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; }
        public int SongNoteId { get; }
        public UpdateNoteDto Command { get; }

        public UpdateSongNoteCommand(int songId, int songVoiceId, int songBarId, int songNoteId, UpdateNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            SongNoteId = songNoteId;
            Command = command;
        }
    }

    public class UpdateSongNoteCommandHandler : IRequestHandler<UpdateSongNoteCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly AuthService _authService;

        public UpdateSongNoteCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongNoteCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

            var note = part.Notes.FirstOrDefault(n => n.Id == request.SongNoteId);
            if (note == null)
            {
                throw new NotFoundException($"Note with Id {request.SongNoteId} not found");
            }

            note.Length = request.Command.Length;
            note.NoteNumber = request.Command.NoteNumber;
            note.SetNoteValues(request.Command.Notes);

            part.SongVoice.SetSongVoiceUpdated(_authService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}