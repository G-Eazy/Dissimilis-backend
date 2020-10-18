using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
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

        public DeleteSongCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedSongCommandDto> Handle(DeleteSongCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongByIdForUpdate(request.SongId, cancellationToken);

            await _repository.DeleteSong(song, cancellationToken);

            return null;
        }
    }
}