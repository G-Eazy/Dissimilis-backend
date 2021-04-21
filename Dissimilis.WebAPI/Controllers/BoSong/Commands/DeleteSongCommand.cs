using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
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
        private readonly Repository _repository;
        private readonly IAuthService _authService;

        public DeleteSongCommandHandler(Repository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(DeleteSongCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var song = await _repository.GetSongByIdForUpdate(request.SongId, cancellationToken);

            if(song.ArrangerId != currentUser.Id)
            {
                throw new UnauthorizedAccessException();
            }

            await _repository.DeleteSong(song, cancellationToken);

            return null;
        }
    }
}