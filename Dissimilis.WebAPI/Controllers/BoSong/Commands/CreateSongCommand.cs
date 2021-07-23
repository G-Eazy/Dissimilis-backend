using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class CreateSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public CreateSongDto Command { get; }

        public CreateSongCommand(CreateSongDto command)
        {
            Command = command;
        }
    }

    public class CreateSongCommandHandler : IRequestHandler<CreateSongCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public CreateSongCommandHandler(SongRepository songRepository, IAuthService authService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedSongCommandDto> Handle(CreateSongCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = new Song()
            {
                Title = request.Command.Title,
                Numerator = request.Command.Numerator,
                Denominator = request.Command.Denominator,
                ArrangerId = currentUser.Id,
                ProtectionLevel = ProtectionLevels.Private
            };
            if(!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Create, cancellationToken)) throw new UnauthorizedAccessException();

            song.SetCreated(currentUser.Id);
            await _songRepository.SaveAsync(song, cancellationToken);

            var mainVoice = new SongVoice()
            {
                SongId = song.Id,
                VoiceNumber = 1,
                VoiceName = "Main",
                IsMainVoice = true,
                SongBars = new SongBar[] { new SongBar() { Position = 1 } }
            };

            song.Voices.Add(mainVoice);
            mainVoice.SetCreated(currentUser.Id);
            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}