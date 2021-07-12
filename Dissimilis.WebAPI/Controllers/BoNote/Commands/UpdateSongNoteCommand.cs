using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoNote.Commands
{
    public class UpdateSongNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongChordId { get; }
        public UpdateNoteDto Command { get; }

        public UpdateSongNoteCommand(int songChordId, UpdateNoteDto command)
        {
            SongChordId = songChordId;
            Command = command;
        }
    }

    public class UpdateSongNoteCommandHandler : IRequestHandler<UpdateSongNoteCommand, UpdatedCommandDto>
    {
        private readonly NoteRepository _noteRepository;
        private readonly IAuthService _IAuthService;

        public UpdateSongNoteCommandHandler(NoteRepository noteRepository, IAuthService IAuthService)
        {
            _noteRepository = noteRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongNoteCommand request, CancellationToken cancellationToken)
        {
            var songNote = await _noteRepository.GetSongNoteById(request.SongChordId, cancellationToken);

            if (songNote == null)
            {
                throw new NotFoundException($"Chord with Id {request.SongChordId} not found");
            }

            songNote.Length = request.Command?.Length ?? songNote.Length;
            songNote.Position = request.Command?.Position ?? songNote.Position;
            songNote.ChordName = request.Command?.ChordName ?? songNote.ChordName;
            songNote.SetNoteValues(request.Command.Notes);

            songNote.SongBar.SongVoice.SetSongVoiceUpdated(_IAuthService.GetVerifiedCurrentUser().Id);

            await _noteRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songNote);
        }
    }
}