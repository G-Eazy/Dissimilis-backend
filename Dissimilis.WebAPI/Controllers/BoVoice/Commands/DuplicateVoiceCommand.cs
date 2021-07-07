using System;
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
    public class DuplicateVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }

        public CreateSongVoiceDto Command { get; set; }

        public DuplicateVoiceCommand(int songId, int songVoiceId, CreateSongVoiceDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class DuplicateVoiceCommandHandler : IRequestHandler<DuplicateVoiceCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly AuthService _IAuthService;

        public DuplicateVoiceCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _IAuthService = authService;
        }

        public async Task<UpdatedCommandDto> Handle(DuplicateVoiceCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongById(request.SongId, cancellationToken);
            var songVoice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }

            var user = _IAuthService.GetVerifiedCurrentUser();

            await using var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            if (string.IsNullOrEmpty(request.Command.VoiceName))
            {
                throw new Exception("Voicename can't be a empty string");
            }

            var duplicatedVoice = songVoice.Clone(request.Command.VoiceName, user, null, song.Voices.Max(v => v.VoiceNumber));
            song.Voices.Add(duplicatedVoice);

            try
            {
                await _repository.UpdateAsync(song, _IAuthService.GetVerifiedCurrentUser(), cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }


            return new UpdatedCommandDto(duplicatedVoice);
        }
    }
}