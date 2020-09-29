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
        private readonly AuthService _authService;

        public CreatePartCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            SongVoice songVoice = null;

            await using (var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
            {
                var song = await _repository.GetSongById(request.SongId, cancellationToken);

                if (song.Voices.Any(v => v.VoiceNumber == request.Command.VoiceNumber))
                {
                    throw new ValidationException("Voice number already used");
                }
                
                var instrument = await _repository.CreateOrFindInstrument(request.Command.Insturment, cancellationToken);

                songVoice = new SongVoice()
                {
                    VoiceNumber = request.Command.VoiceNumber,
                    Instrument = instrument,
                    Song = song
                };


                song.Voices.Add(songVoice);

                song.SyncBarCountToMaxInAllVoices();
                song.UpdateAllSongVoices(currentUser.Id);

                await _repository.UpdateAsync(cancellationToken);

                try
                {
                    await _repository.UpdateAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new ValidationException("Transaction error happend, aborting operation. Please try again.");
                }
            }

            return new UpdatedCommandDto(songVoice);
        }
    }
}