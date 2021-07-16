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

        public AddComponentIntervalNoteHandler(SongRepository songRepository, IAuthService authService)
        {
            _songRepository = songRepository;
            _authService = authService;
        }
        public async Task<UpdatedCommandDto> Handle(AddComponentIntervalNoteCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _songRepository.HasWriteAccess(song, currentUser)) throw new UnauthorizedAccessException();

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

                songNote = new SongNote()
                {
                    ChordName = request.Command.ChordName,
                    NoteValues = String.Join("|", Enumerable.Repeat("X", SongNoteExtension.GetNoteValuesFromChordName(request.Command.ChordName).Count)),
                    Position = request.Command.NotePosition,
                    Length = request.Command.Length,
                };
            }

            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            songNote.AddComponentInterval(request.Command.IntervalPosition);
            try
            {
                await _songRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }

            return new UpdatedCommandDto(songBar);
        }
    }
}
