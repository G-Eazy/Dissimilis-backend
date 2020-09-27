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
    public class DeleteBarCommand : IRequest<UpdatedBarCommandDto>
    {
        public int SongId { get; }
        public int PartId { get; set; }
        public int BarId { get; }

        public DeleteBarCommand(int songId, int partId, int barId)
        {
            SongId = songId;
            PartId = partId;
            BarId = barId;
        }
    }

    public class DeleteBarCommandHandler : IRequestHandler<DeleteBarCommand, UpdatedBarCommandDto>
    {
        private readonly Repository _repository;

        public DeleteBarCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedBarCommandDto> Handle(DeleteBarCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetPartById(request.SongId, request.PartId, cancellationToken);

            var bar = part.Bars.FirstOrDefault(b => b.Id == request.BarId);
            if (bar == null)
            {
                throw new NotFoundException($"Bar with Id {request.BarId} was not found");
            }

            part.Bars.Remove(bar);
            part.SortBars();
            await _repository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}