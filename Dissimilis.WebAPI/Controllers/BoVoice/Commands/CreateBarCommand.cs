using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class CreateBarCommand : IRequest<UpdatedBarCommandDto>
    {
        public int SongId { get; }
        public CreateBarDto Command { get; }
        public int PartId { get; set; }

        public CreateBarCommand(int songId, int partId, CreateBarDto command)
        {
            SongId = songId;
            Command = command;
            PartId = partId;
        }
    }

    public class CreateBarCommandHandler : IRequestHandler<CreateBarCommand, UpdatedBarCommandDto>
    {
        private readonly Repository _repository;

        public CreateBarCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedBarCommandDto> Handle(CreateBarCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetPartById(request.SongId, request.PartId, cancellationToken);

            var bar = new SongBar()
            {
                BarNumber = request.Command.BarNumber,
                RepAfter = request.Command.RepAfter,
                RepBefore = request.Command.RepBefore,
                House = request.Command.House
            };

            // Add/update for all parts of the song. 

            var barsToIncrease = part.Bars.Where(b => b.BarNumber >= bar.BarNumber);
            foreach (var songBar in barsToIncrease)
            {
                songBar.BarNumber++;
            }
            part.Bars.Add(bar);

            part.SortBars();

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedBarCommandDto(bar);
        }
    }
}