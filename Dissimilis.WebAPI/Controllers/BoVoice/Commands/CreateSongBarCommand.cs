using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class CreateSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public CreateBarDto Command { get; }
        public int SongVoiceId { get; set; }

        public CreateSongBarCommand(int songId, int songVoiceId, CreateBarDto command)
        {
            SongId = songId;
            Command = command;
            SongVoiceId = songVoiceId;
        }
    }

    public class CreateSongBarCommandHandler : IRequestHandler<CreateSongBarCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;

        public CreateSongBarCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongBarCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetSongPartById(request.SongId, request.SongVoiceId, cancellationToken);

            var bar = new SongBar()
            {
                BarNumber = request.Command.BarNumber,
                RepAfter = request.Command.RepAfter,
                RepBefore = request.Command.RepBefore,
                House = request.Command.House
            };

            // Add/update for all parts of the song. 
            // TODO Add last, count to ensure all songVoices of the song have the same amount of bars
            var barsToIncrease = part.SongBars.Where(b => b.BarNumber >= bar.BarNumber);
            foreach (var songBar in barsToIncrease)
            {
                songBar.BarNumber++;
            }
            part.SongBars.Add(bar);
            part.SortBars();


            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(bar);
        }
    }
}