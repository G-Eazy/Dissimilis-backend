using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Exceptions;
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
        public int SongChordId { get; }
        public UpdateNoteDto Command { get; }

        public UpdateSongNoteCommand(int songId, int songVoiceId, int songBarId, int songChordId, UpdateNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            SongChordId = songChordId;
            Command = command;
        }
    }

    public class UpdateSongNoteCommandHandler : IRequestHandler<UpdateSongNoteCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public UpdateSongNoteCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongNoteCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongById(request.SongId, cancellationToken);
            var part = await _repository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

            var note = part.Notes.FirstOrDefault(n => n.Id == request.SongChordId);
            if (note == null)
            {
                throw new NotFoundException($"Chord with Id {request.SongChordId} not found");
            }

            note.Length = request.Command.Length;
            note.Position = request.Command.Position;
            note.ChordName = request.Command.ChordName;
            note.SetNoteValues(request.Command.Notes);


            part.SongVoice.SetSongVoiceUpdated(_IAuthService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(song, _IAuthService.GetVerifiedCurrentUser(), cancellationToken);

            return null;
        }
    }
}