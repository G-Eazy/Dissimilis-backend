using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class DuplicateSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public DuplicateSongDto Command { get; }

        public DuplicateSongCommand(int songId, DuplicateSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class DuplicateSongCommandHandler : IRequestHandler<DuplicateSongCommand, UpdatedSongCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _authService;

        public DuplicateSongCommandHandler(Repository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(DuplicateSongCommand request, CancellationToken cancellationToken)
        {
            var duplicateFromSong = await _repository.GetFullSongById(request.SongId, cancellationToken);

            var duplicatedSong = duplicateFromSong.Clone(request.Command.Title);

            var currentUser = _authService.GetVerifiedCurrentUser();
            duplicatedSong.SetUpdated(currentUser.Id);

            await _repository.SaveAsync(duplicatedSong, cancellationToken);

            return new UpdatedSongCommandDto(duplicatedSong);
        }
    }
}