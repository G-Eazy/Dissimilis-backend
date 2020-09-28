using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class DeleteSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }

        public DeleteSongVoiceCommand(int songId, int songVoiceId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
        }
    }

    public class DeleteSongVoiceCommandHandle : IRequestHandler<DeleteSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;

        public DeleteSongVoiceCommandHandle(Repository repository)
        {
            _repository = repository;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongById(request.SongId, cancellationToken);

            var voice = song.Voices.FirstOrDefault(sv => sv.Id == request.SongVoiceId);
            if (voice == null)
            {
                throw new NotFoundException($"Voice with Id {request.SongVoiceId} not found");
            }

            song.Voices.Remove(voice);

            await _repository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}