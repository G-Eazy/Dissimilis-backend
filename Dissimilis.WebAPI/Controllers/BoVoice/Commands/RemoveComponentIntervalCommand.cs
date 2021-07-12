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

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class RemoveComponentIntervalCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public RemoveComponentIntervalDto Command { get; }

        public RemoveComponentIntervalCommand(int songId, int songVoiceId, RemoveComponentIntervalDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class RemoveComponentIntervalHandler : IRequestHandler<RemoveComponentIntervalCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly VoiceRepository _voiceRepository;
        private readonly AuthService _authService;

        public RemoveComponentIntervalHandler(SongRepository songRepository, VoiceRepository voiceRepository, AuthService authService)
        {
            _songRepository = songRepository;
            _voiceRepository = voiceRepository;
            _authService = authService;
        }
        public async Task<UpdatedCommandDto> Handle(RemoveComponentIntervalCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
            var user = _authService.GetVerifiedCurrentUser();
            song.PerformSnapshot(user);

            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }

            await using var transaction = await _voiceRepository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            songVoice.RemoveComponentInterval(request.Command.IntervalPosition);
            songVoice.SetSongVoiceUpdated(_authService.GetVerifiedCurrentUser().Id);
            try
            {
                await _voiceRepository.UpdateAsync(cancellationToken);
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
