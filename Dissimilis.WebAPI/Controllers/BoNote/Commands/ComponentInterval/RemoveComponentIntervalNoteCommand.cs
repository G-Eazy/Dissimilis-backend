using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoNote.Commands.ComponentInterval
{
    public class RemoveComponentIntervalNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; set; }
        public int SongVoiceId { get; set; }
        public int SongBarPosition { get; set; }
        public RemoveComponentIntervalNoteDto Command { get; set; }

        public RemoveComponentIntervalNoteCommand(int songId, int songVoiceId, int songBarPosition, RemoveComponentIntervalNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarPosition = songBarPosition;
            Command = command;
        }
    }

    public class RemoveComponentIntervalNoteCommandHandler : IRequestHandler<RemoveComponentIntervalNoteCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly AuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public RemoveComponentIntervalNoteCommandHandler(SongRepository songRepository, AuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _authService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }
        public async Task<UpdatedCommandDto> Handle(RemoveComponentIntervalNoteCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();
            

            var songVoice = song.Voices.SingleOrDefault(v => v.Id == request.SongVoiceId);
            if (songVoice == null)throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");

            var songBar = songVoice.SongBars.SingleOrDefault(bar => bar.Position == request.SongBarPosition);
            if (songBar == null) throw new NotFoundException($"Bar with position {request.SongBarPosition} not found");

            var songNote = songBar.Notes.SingleOrDefault(note => note.Position == request.Command.NotePosition);
            if (songNote == null) throw new NotFoundException($"Note with position {request.Command.NotePosition} not found");
            song.PerformSnapshot(currentUser);

            if (songNote.ChordName == null)
            {
                songBar.Notes.Remove(songNote);
            }
            else
            {
                songNote.RemoveComponentInterval(request.Command.IntervalPosition);
                if (songNote.NoteValues.Split("|").All(note => note == "X") && request.Command.DeleteOnLastIntervalRemoved)
                    songBar.Notes.Remove(songNote);
            }

            songBar.SongVoice.SetSongVoiceUpdated(currentUser.Id);

            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songBar);
        }
    }
}
