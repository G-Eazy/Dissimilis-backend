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
    public class ChangeProtectionLevelSongCommand : IRequest<ProtectionLevelSongDto>
    {
        public int SongId { get; }
        public UpdateProtectionLevelDto Command{ get; }
        public ChangeProtectionLevelSongCommand(UpdateProtectionLevelDto command, int songId)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class ChangeProtectionLevelSongCommandHandler : IRequestHandler<ChangeProtectionLevelSongCommand, ProtectionLevelSongDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;

        public ChangeProtectionLevelSongCommandHandler(SongRepository songRepository, IAuthService authService)
        {
            _songRepository = songRepository;
            _authService = authService;
        }

        public async Task<ProtectionLevelSongDto> Handle(ChangeProtectionLevelSongCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if( currentUser.Id != song.ArrangerId)
            {
                throw new UnauthorizedAccessException();
            }
            switch (request.Command.ProtectionLevel)
            {
                case "Public":
                    song.ProtectionLevel = ProtectionLevels.Public;
                    break;

                case "Private": song.ProtectionLevel = ProtectionLevels.Private;
                        break;
                default:
                    throw new Exception("Protectionlevel need to be either Private or Public");
            }

            await _songRepository.UpdateAsync(cancellationToken);

            return new ProtectionLevelSongDto(song);
        }
    }
}