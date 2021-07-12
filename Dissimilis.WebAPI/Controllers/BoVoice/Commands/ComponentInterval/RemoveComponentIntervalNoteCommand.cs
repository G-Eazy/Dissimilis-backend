using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class RemoveComponentIntervalNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; set; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; set; }
        public int SongNoteId { get; set; }
        public RemoveComponentIntervalNoteDto Command { get; set; }

        public RemoveComponentIntervalNoteCommand(int songId, int songVoiceId, int songBarId, int songNoteId, RemoveComponentIntervalNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            SongNoteId = songNoteId;
            Command = command;
        }
    }

    public class RemoveComponentIntervalNoteCommandHandler : IRequestHandler<RemoveComponentIntervalNoteCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly AuthService _authService;
        public RemoveComponentIntervalNoteCommandHandler(SongRepository songRepository, AuthService IAuthService)
        {
            _songRepository = songRepository;
            _authService = IAuthService;
        }
        public async Task<UpdatedCommandDto> Handle(RemoveComponentIntervalNoteCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
            if (song == null)
            {
                throw new NotFoundException($"Song with id {request.SongId} not found");
            }
            var songVoice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }
            var songBar = songVoice.SongBars.FirstOrDefault(bar => bar.Id == request.SongBarId);
            if (songBar == null)
            {
                throw new NotFoundException($"Bar with id {request.SongBarId} not found");
            }
            var songNote = songBar.Notes.FirstOrDefault(note => note.Id == request.SongNoteId);
            if (songBar == null)
            {
                throw new NotFoundException($"Note with id {request.SongNoteId} not found");
            }

            var user = _authService.GetVerifiedCurrentUser();

            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            songNote.RemoveComponentInterval(request.Command.IntervalPosition);
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


            return new UpdatedCommandDto(songVoice);
        }
    }
}
