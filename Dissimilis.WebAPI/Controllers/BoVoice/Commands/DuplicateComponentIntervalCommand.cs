using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class DuplicateComponentIntervalCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SourceVoiceId { get; set; }
        public int IntervalPosition { get; set; }

        public DuplicateComponentIntervalCommand(int songId, int songVoiceId, int sourceVoiceId, int intervalPosition)
        {
            this.SongId = songId;
            this.SongVoiceId = songVoiceId;
            this.SourceVoiceId = sourceVoiceId;
            this.IntervalPosition = intervalPosition;
        }
    }

    public class DuplicateVoiceCommandHandler : IRequestHandler<DuplicateComponentIntervalCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly AuthService _authService;

        public DuplicateVoiceCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }
        public async Task<UpdatedCommandDto> Handle(DuplicateComponentIntervalCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongById(request.SongId, cancellationToken);
            var songVoice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }
            var sourceVoice = song.Voices.FirstOrDefault(v => v.Id == request.SourceVoiceId);
            if (sourceVoice == null)
            {
                throw new NotFoundException($"Source voice with id {request.SourceVoiceId} not found");
            }

            var user = _authService.GetVerifiedCurrentUser();

            await using var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var instrumentName = songVoice.Instrument?.Name.GetNextSongVoiceName();
            var instrument = await _repository.CreateOrFindInstrument(instrumentName, cancellationToken);

            songVoice.CopyComponentInterval(sourceVoice, user, request.IntervalPosition);
            try
            {
                await _repository.UpdateAsync(cancellationToken);
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
