using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Data.IsolationLevel;


namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class CreateSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public CreateBarDto Command { get; }
        public int SongVoiceId { get; set; }

        public CreateSongBarCommand(int songId, int songVoiceId, CreateBarDto command)
        {
            SongId = songId;
            Command = command;
            SongVoiceId = songVoiceId;
        }
    }

    public class CreateSongBarCommandHandler : IRequestHandler<CreateSongBarCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public CreateSongBarCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongBarCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            SongBar songBar = null;
            await using (var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
            {
                var song = await _repository.GetSongById(request.SongId, cancellationToken);

                var voice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
                if (voice == null)
                {
                    throw new NotFoundException($"Voice with Id {voice.Id} not fond");
                }

                songBar = new SongBar()
                {
                    Position = voice.SongBars.OrderByDescending(sb => sb.Position).FirstOrDefault()?.Position + 1 ?? 1,
                    RepAfter = request.Command.RepAfter,
                    RepBefore = request.Command.RepBefore,
                    House = request.Command.House
                };

                voice.SongBars.Add(songBar);
                song.SyncBarCountToMaxInAllVoices();

                song.UpdateAllSongVoices(currentUser.Id);

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

            return new UpdatedCommandDto(songBar);
        }
    }
}