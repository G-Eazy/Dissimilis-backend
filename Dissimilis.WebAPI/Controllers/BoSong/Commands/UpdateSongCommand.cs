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
        private readonly AuthService _authService;

        public UpdateSongCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(UpdateSongCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongByIdForUpdate(request.SongId, cancellationToken);

            song.Title = request.Command.Title;
            //song.Numerator = request.Command.Numerator;
            //song.Denominator = request.Command.Denominator;

            song.SetUpdated(_authService.GetVerifiedCurrentUser());

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}