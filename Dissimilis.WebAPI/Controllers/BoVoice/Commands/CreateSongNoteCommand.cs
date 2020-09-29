using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class CreateSongNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; }
        public CreateNoteDto Command { get; }

        public CreateSongNoteCommand(int songId, int songVoiceId, int songBarId, CreateNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            Command = command;
        }
    }

    public class CreateSongNoteCommandHandler : IRequestHandler<CreateSongNoteCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;

        public CreateSongNoteCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongNoteCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

            if (part.Notes.Any(n => n.NoteNumber == request.Command.NoteNumber))
            {
                throw new ValidationException("Note number already in use");
            }

            var note = new SongNote()
            {
                NoteNumber = request.Command.NoteNumber,
                Length = request.Command.Length
            };

            note.SetNoteValues(request.Command.Notes);

            part.Notes.Add(note);

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(note);
        }
    }
}