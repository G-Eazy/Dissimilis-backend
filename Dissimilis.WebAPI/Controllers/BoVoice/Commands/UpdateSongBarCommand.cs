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


namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class UpdateSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        public int BarId { get; }
        public UpdateBarDto Command { get; }

        public UpdateSongBarCommand(int songId, int songVoiceId, int barId, UpdateBarDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            BarId = barId;
            Command = command;
        }
    }

    public class UpdateSongBarCommandHandler : IRequestHandler<UpdateSongBarCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public UpdateSongBarCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongBarCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var song = await _repository.GetSongById(request.SongId, cancellationToken);

            var voice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (voice == null)
            {
                throw new NotFoundException($"Voice with Id {request.SongVoiceId} not found");
            }

            var bar = voice.SongBars.FirstOrDefault(b => b.Id == request.BarId);
            if (bar == null)
            {
                throw new NotFoundException($"Bar with Id {request.BarId} not found");
            }

            bar.RepAfter = request.Command.RepAfter;
            bar.RepBefore = request.Command.RepBefore;
            bar.House = request.Command.House;
            if (bar.House == 0)
            {
                bar.House = null;
            }

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);
            song.SyncVoicesFrom(voice);

            try
            {
                await _repository.UpdateAsync(song, currentUser, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }

            return new UpdatedCommandDto(bar);
        }
    }
}