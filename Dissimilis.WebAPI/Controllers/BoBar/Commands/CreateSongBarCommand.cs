using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IsolationLevel = System.Data.IsolationLevel;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;

namespace Dissimilis.WebAPI.Controllers.BoBar.Commands
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
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public CreateSongBarCommandHandler(BarRepository barRepository, SongRepository songRepository, IAuthService IAuthService)
        {
            _barRepository = barRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongBarCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            SongBar songBar = null;
            await using (var transaction = await _barRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
            {
                var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
                song.PerformSnapshot(currentUser);


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

                voice.SongBars = voice.SongBars.Concat(new[] { songBar }).ToArray();
                song.SyncVoicesFrom(voice);

                song.SetUpdatedOverAll(currentUser.Id);

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
            }

            return new UpdatedCommandDto(songBar);
        }
    }
}
