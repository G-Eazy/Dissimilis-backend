using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class CreateSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public CreateSongVoiceDto Command { get; }

        public CreateSongVoiceCommand(int songId, CreateSongVoiceDto command)
        {
            SongId = songId;
            Command = command;

        }
    }

    public class CreatePartCommandHandler : IRequestHandler<CreateSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public CreatePartCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            await using var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var song = await _repository.GetSongById(request.SongId, cancellationToken);

            if (song.Voices.Any(v => v.VoiceNumber == request.Command.VoiceNumber))
            {
                throw new ValidationException("Voice number already used");
            }
            if (string.IsNullOrEmpty(request.Command.VoiceName))
            {
                throw new ValidationException("VoiceName not defined");
            }

            var nextVoiceNumber = song.Voices.OrderByDescending(v => v.VoiceNumber).FirstOrDefault()?.VoiceNumber ?? 0;
            nextVoiceNumber++;

            var songVoice = new SongVoice()
            {
                VoiceNumber = request.Command.VoiceNumber ?? nextVoiceNumber,
                Instrument = null,
                Song = song,
                VoiceName = request.Command.VoiceName
            };

            var cloneVoice = song.Voices.FirstOrDefault();
            song.Voices.Add(songVoice);

            song.SyncBarCountToMaxInAllVoices();
            song.SetUpdatedOverAll(currentUser.Id);

            if (cloneVoice != null)
            {
                song.SyncVoicesFrom(cloneVoice);
            }

            await _repository.UpdateAsync(song, currentUser, cancellationToken);

            try
            {
                await _repository.UpdateAsync(song, currentUser, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }

            return new UpdatedCommandDto(songVoice);
        }
    }
}