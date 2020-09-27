using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class UpdateBarCommand : IRequest<UpdatedBarCommandDto>
    {
        public int SongId { get; }
        public int PartId { get; }
        public int BarId { get; }
        public UpdateBarDto Command { get; }

        public UpdateBarCommand(int songId, int partId, int barId, UpdateBarDto command)
        {
            SongId = songId;
            PartId = partId;
            BarId = barId;
            Command = command;
        }
    }

    public class UpdateBarCommandHandler : IRequestHandler<UpdateBarCommand, UpdatedBarCommandDto>
    {
        private readonly Repository _repository;

        public UpdateBarCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedBarCommandDto> Handle(UpdateBarCommand request, CancellationToken cancellationToken)
        {
            var bar = await _repository.GetBarById(request.SongId, request.PartId, request.BarId, cancellationToken);

            bar.RepAfter = request.Command.RepAfter;
            bar.RepBefore = request.Command.RepBefore;
            bar.House = request.Command.House;

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedBarCommandDto(bar);
        }
    }
}