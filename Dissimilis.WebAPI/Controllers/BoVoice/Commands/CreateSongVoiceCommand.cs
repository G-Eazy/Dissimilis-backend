using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class CreateSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public CreateSongVoiceDto Command { get; }

        public CreateSongVoiceCommand(int songId, CreateSongVoiceDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class CreatePartCommandHandler : IRequestHandler<CreateSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;

        public CreatePartCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongById(request.SongId, cancellationToken);
            var instrument = await _repository.CreateOrFindInstrument(request.Command.Insturment, cancellationToken);

            var songVoice = new SongVoice()
            {
                VoiceNumber = request.Command.VoiceNumber,
                Instrument = instrument
            };

            // Get same amount of bars for this song as for the "main part"
            // TODO update so all parts have the same amount of parts

            song.Voices.Add(songVoice);

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}