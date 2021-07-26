using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong;
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
        public int SongId { get; }
        public UpdateNoteDto Command { get; }

        public UpdateSongNoteCommand(int songId, int songChordId, UpdateNoteDto command)
        {
            SongChordId = songChordId;
            SongId = songId;
            Command = command;
        }
    }

    public class UpdateSongNoteCommandHandler : IRequestHandler<UpdateSongNoteCommand, UpdatedCommandDto>
    {
        private readonly NoteRepository _noteRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public UpdateSongNoteCommandHandler(NoteRepository noteRepository, SongRepository songRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _noteRepository = noteRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongNoteCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();


            var songNote = await _noteRepository.GetSongNoteById(request.SongChordId, cancellationToken);

            if (songNote == null)
            {
                throw new NotFoundException($"Chord with Id {request.SongChordId} not found");
            }

            songNote.Length = request.Command?.Length ?? songNote.Length;
            songNote.Position = request.Command?.Position ?? songNote.Position;
            songNote.ChordName = request.Command.ChordName;
            if (songNote.ChordName != null)
            {
                songNote.SetNoteValues(SongNoteExtension.GetNoteValuesFromChordName(songNote.ChordName).ToArray());
            }
            else
            {
                songNote.SetNoteValues(request.Command.Notes);
            }

            songNote.SongBar.SongVoice.SetSongVoiceUpdated(_IAuthService.GetVerifiedCurrentUser().Id);

            await _noteRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songNote);
        }
    }
}