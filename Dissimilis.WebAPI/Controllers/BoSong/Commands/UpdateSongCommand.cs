using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class UpdateSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public UpdateSongDto Command { get; }


        public UpdateSongCommand(int songId, UpdateSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class UpdateSongCommandHandler : IRequestHandler<UpdateSongCommand, UpdatedSongCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public UpdateSongCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(UpdateSongCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongByIdForUpdate(request.SongId, cancellationToken);

            song.Title = request.Command.Title;

            song.SetUpdated(_IAuthService.GetVerifiedCurrentUser());

            await _repository.UpdateAsync(song, user, cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}