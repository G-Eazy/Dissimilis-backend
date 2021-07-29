using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoNote.Commands.ComponentInterval
{
    public class AddComponentIntervalNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SongBarPosition { get; set; }
        public AddComponentIntervalNoteDto Command { get; }

        public AddComponentIntervalNoteCommand(int songId, int songVoiceId, int songBarPosition, AddComponentIntervalNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarPosition = songBarPosition;
            Command = command;
        }
    }

    public class AddComponentIntervalNoteHandler : IRequestHandler<AddComponentIntervalNoteCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public AddComponentIntervalNoteHandler(SongRepository songRepository, IAuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }
        public async Task<UpdatedCommandDto> Handle(AddComponentIntervalNoteCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
            var songVoice = song.Voices.SingleOrDefault(voice => voice.Id == request.SongVoiceId);
            if (songVoice == null) throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");

            var songBar = songVoice.SongBars.SingleOrDefault(bar => bar.Position == request.SongBarPosition);
            if (songBar == null) throw new NotFoundException($"Bar with position {request.SongBarPosition} not found");

            var songNote = songBar.Notes.SingleOrDefault(note => note.Position == request.Command.NotePosition);
            if (songNote == null)
            {
                if (songBar.Notes.Any(note => {
                    var (startPos, endPos) = note.GetNotePositionRange();
                    return startPos >= request.Command.NotePosition && request.Command.NotePosition >= endPos;}))
                { throw new ValidationException("Note number already in use"); }
                song.PerformSnapshot(currentUser);

                var chordName = request.Command.ChordName == ""
                    ? null
                    : request.Command.ChordName;

                songNote = new SongNote()
                {
                    ChordName = chordName,
                    NoteValues = chordName == null
                        ? String.Join("|", request.Command.Notes)
                        : String.Join("|", Enumerable.Repeat("X", SongNoteExtension.GetNoteValuesFromChordName(request.Command.ChordName).Count)),
                    Position = request.Command.NotePosition,
                    Length = request.Command.Length,
                };

                songBar.Notes.Add(songNote);
            }

            if (songNote.ChordName != null)
                songNote.AddComponentInterval(request.Command.IntervalPosition);

            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songBar);
        }
    }
}
