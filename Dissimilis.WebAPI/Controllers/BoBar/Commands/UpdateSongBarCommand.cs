using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;

namespace Dissimilis.WebAPI.Controllers.BoBar.Commands
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
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public UpdateSongBarCommandHandler(BarRepository barRepository, SongRepository songRepository, IAuthService IAuthService)
        {
            _barRepository = barRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongBarCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
            await using var transaction = await _barRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

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
            song.PerformSnapshot(_IAuthService.GetVerifiedCurrentUser());

            bar.RepAfter = request.Command?.RepAfter ?? bar.RepAfter;
            bar.RepBefore = request.Command?.RepBefore ?? bar.RepBefore;
            bar.House = request.Command?.House ?? bar.House;
            if (bar.House == 0)
            {
                bar.House = null;
            }

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);
            song.SyncVoicesFrom(voice);

            try
            {
                await _barRepository.UpdateAsync(cancellationToken);
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
