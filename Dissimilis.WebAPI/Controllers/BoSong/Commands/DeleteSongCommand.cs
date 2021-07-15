using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class DeleteSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }

        public DeleteSongCommand(int songId)
        {
            SongId = songId;
        }
    }

    public class DeleteSongCommandHandler : IRequestHandler<DeleteSongCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;

        public DeleteSongCommandHandler(SongRepository songRepository, IAuthService authService)
        {
            _songRepository = songRepository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(DeleteSongCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);

            if(! await _songRepository.HasWriteAccess(song, currentUser))
            {
                throw new UnauthorizedAccessException();
            }

            await _songRepository.DeleteSong(currentUser, song, cancellationToken);
            song.SetUpdated(currentUser);

            return null;
        }
    }
}