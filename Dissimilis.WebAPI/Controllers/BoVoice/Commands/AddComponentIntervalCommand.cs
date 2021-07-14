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
    public class AddComponentIntervalCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public AddComponentIntervalDto Command { get; }

        public AddComponentIntervalCommand(int songId, int songVoiceId, AddComponentIntervalDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class AddComponentIntervalHandler : IRequestHandler<AddComponentIntervalCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly VoiceRepository _voiceRepository;
        private readonly AuthService _authService;

        public AddComponentIntervalHandler(SongRepository songRepository, VoiceRepository voiceRepository, AuthService authService)
        {
            _songRepository = songRepository;
            _voiceRepository = voiceRepository;
            _authService = authService;
        }
        public async Task<UpdatedCommandDto> Handle(AddComponentIntervalCommand request, CancellationToken cancellationToken)
        {
            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }

            var user = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);

            await using var transaction = await _voiceRepository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            song.PerformSnapshot(user);

            songVoice.AddComponentInterval(request.Command.IntervalPosition);
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
