using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class CreateTransposedSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public TransposeSongDto Command { get; }

        public CreateTransposedSongCommand(int songId, TransposeSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class CreateTransposedSongCommandHandler : IRequestHandler<CreateTransposedSongCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;

        public CreateTransposedSongCommandHandler(SongRepository songRepository, IAuthService authService)
        {
            _songRepository = songRepository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(CreateTransposedSongCommand request, CancellationToken cancellationToken)
        {
            // Do we have to get the fullsong to transpose
            var duplicateFromSong = await _songRepository.GetFullSongById(request.SongId, cancellationToken);

            var duplicatedSong = duplicateFromSong.Clone(request.Command.Title);
            duplicatedSong = duplicatedSong.Transpose(request.Command.Transpose);

            duplicatedSong.SetUpdated(_authService.GetVerifiedCurrentUser());
            await _songRepository.SaveAsync(duplicatedSong, cancellationToken);

            return new UpdatedSongCommandDto(duplicatedSong);
        }
    }
}
