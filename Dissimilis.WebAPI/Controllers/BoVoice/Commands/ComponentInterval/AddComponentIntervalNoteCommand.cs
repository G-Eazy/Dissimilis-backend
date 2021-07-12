using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands.ComponentInterval
{
    public class AddComponentIntervalNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; set; }
        public int SongNoteId { get; set; }
        public AddComponentIntervalNoteDto Command { get; }

        public AddComponentIntervalNoteCommand(int songId, int songVoiceId, int songBarId, int songNoteId, AddComponentIntervalNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            SongNoteId = songNoteId;
            Command = command;
        }
    }

    public class AddComponentIntervalNoteHandler : IRequestHandler<AddComponentIntervalNoteCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly Repository _repository;
        private readonly IAuthService _IAuthUserService;

        public AddComponentIntervalNoteHandler(SongRepository songRepository, IAuthService authService)
        {
            _songRepository = songRepository;
            _IAuthUserService = authService;
        }
        public async Task<UpdatedCommandDto> Handle(AddComponentIntervalNoteCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
            song.PerformSnapshot(_IAuthUserService.GetVerifiedCurrentUser());

            var songVoice = song.Voices.SingleOrDefault(voice => voice.Id == request.SongVoiceId);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }
            var songBar = songVoice.SongBars.SingleOrDefault(bar => bar.Id == request.SongBarId);
            if (songBar == null)
            {
                throw new NotFoundException($"Bar with id {request.SongBarId} not found");
            }
            var songNote = songBar.Notes.SingleOrDefault(note => note.Id == request.SongNoteId);
            if (songNote == null)
            {
                throw new NotFoundException($"Note with id {request.SongNoteId} not found");
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

            return new UpdatedCommandDto(songVoice);
        }
    }
}
