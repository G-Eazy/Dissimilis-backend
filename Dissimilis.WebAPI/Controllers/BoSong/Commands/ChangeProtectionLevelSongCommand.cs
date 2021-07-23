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
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public ChangeProtectionLevelSongCommandHandler(SongRepository songRepository, IAuthService authService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }


        public async Task<ProtectionLevelSongDto> Handle(ChangeProtectionLevelSongCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            song.ProtectionLevel = request.Command.ProtectionLevel switch
            {
                "Public" => ProtectionLevels.Public,
                "Private" => ProtectionLevels.Private,
                _ => throw new Exception("Protectionlevel need to be either Private or Public"),
            };
            await _songRepository.UpdateAsync(cancellationToken);

            return new ProtectionLevelSongDto(song);
        }
    }
}