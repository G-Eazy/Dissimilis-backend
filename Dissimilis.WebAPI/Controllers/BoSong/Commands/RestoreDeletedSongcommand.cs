using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class RestoreDeletedSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; set; }

        public RestoreDeletedSongCommand(int songId)
        {
            SongId = songId;
        }
    }

    public class RestoreDeletedSongCommandHandler : IRequestHandler<RestoreDeletedSongCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;

        public RestoreDeletedSongCommandHandler(SongRepository songRepository, IAuthService authService)
        {
            _songRepository = songRepository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(RestoreDeletedSongCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();

            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
            if (!await _songRepository.HasWriteAccess(song, currentUser))
            {
                throw new System.UnauthorizedAccessException();
            }

            await _songRepository.RestoreSong(song, cancellationToken);
            song.SetUpdated(currentUser);

            return new UpdatedSongCommandDto(song);
        }
    }
}