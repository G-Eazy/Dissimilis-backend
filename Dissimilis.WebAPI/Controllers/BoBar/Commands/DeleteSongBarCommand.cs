using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IsolationLevel = System.Data.IsolationLevel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;

namespace Dissimilis.WebAPI.Controllers.BoBar.Commands
{
    public class DeleteSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int BarId { get; }

        public DeleteSongBarCommand(int songId, int songVoiceId, int barId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            BarId = barId;
        }
    }

    public class DeleteSongBarCommandHandler : IRequestHandler<DeleteSongBarCommand, UpdatedCommandDto>
    {
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public DeleteSongBarCommandHandler(BarRepository repository, SongRepository songRepository, IAuthService IAuthService)
        {
            _barRepository = repository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongBarCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            await using var transaction = await _barRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);

            var songVoice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with Id {request.SongVoiceId} was not found");
            }

            var bar = songVoice.SongBars.FirstOrDefault(b => b.Id == request.BarId);
            if (bar == null)
            {
                throw new NotFoundException($"Bar with Id {request.BarId} was not found");
            }
            song.PerformSnapshot(currentUser);

            song.RemoveSongBarFromAllVoices(bar.Position);
            song.SetUpdatedOverAll(currentUser.Id);
            song.SyncVoicesFrom(songVoice);

            try
            {
                await _barRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }

            return null;
        }
    }
}
