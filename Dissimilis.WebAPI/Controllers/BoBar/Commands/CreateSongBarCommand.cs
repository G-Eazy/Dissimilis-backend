using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.DbContext.Models.Enums;
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
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public CreateSongBarCommandHandler(BarRepository barRepository, SongRepository songRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _barRepository = barRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongBarCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            SongBar songBar = null;
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken))throw new UnauthorizedAccessException();

            var voice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
            if (voice == null)
            {
                throw new NotFoundException($"Voice with Id {voice.Id} not fond");
            }
            song.PerformSnapshot(currentUser);
            songBar = new SongBar()
            {
                Position = voice.SongBars.OrderByDescending(sb => sb.Position).FirstOrDefault()?.Position + 1 ?? 1,
                RepAfter = request.Command.RepAfter,
                RepBefore = request.Command.RepBefore,
                VoltaBracket = request.Command.VoltaBracket
            };

            voice.SongBars.Add(songBar);
            song.SyncVoicesFrom(voice);
            song.SetUpdatedOverAll(currentUser.Id);
            await _barRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songBar);
        }
    }
}
