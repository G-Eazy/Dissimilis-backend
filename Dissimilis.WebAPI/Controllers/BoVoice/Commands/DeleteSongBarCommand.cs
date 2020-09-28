using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class DeleteSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int BarId { get; }

        public DeleteSongBarCommand(int songId, int songVoiceId, int barId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            BarId = barId;
        }
    }

    public class DeleteSongBarCommandHandler : IRequestHandler<DeleteSongBarCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;

        public DeleteSongBarCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongBarCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetSongPartById(request.SongId, request.SongVoiceId, cancellationToken);

            var bar = part.SongBars.FirstOrDefault(b => b.Id == request.BarId);
            if (bar == null)
            {
                throw new NotFoundException($"Bar with Id {request.BarId} was not found");
            }

            part.SongBars.Remove(bar);
            part.SortBars();
            await _repository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}