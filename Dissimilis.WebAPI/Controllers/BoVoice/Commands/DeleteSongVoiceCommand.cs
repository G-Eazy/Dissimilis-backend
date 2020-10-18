using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
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
        private readonly IAuthService _IAuthService;

        public DeleteSongVoiceCommandHandle(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
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

            song.SetUpdated(_IAuthService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(cancellationToken);

            return null;
        }
    }
}