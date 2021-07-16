using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class DeleteSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        //public IMediator _mediator { get; set; }

        public DeleteSongVoiceCommand(int songId, int songVoiceId, IMediator mediator)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            //_mediator = mediator;
        }
    }

    public class DeleteSongVoiceCommandHandle : IRequestHandler<DeleteSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly IMediator _mediator;
        private readonly VoiceRepository _voiceRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public DeleteSongVoiceCommandHandle(IMediator mediator, VoiceRepository voiceRepository, SongRepository songRepository, IAuthService IAuthService)
        {
            _mediator = mediator;
            _voiceRepository = voiceRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();

            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with Id {request.SongVoiceId} not found");
            }
            song.PerformSnapshot(currentUser);
            //await _mediator.Send(new PerformSnapshotCommand(request.SongId)); //song.PerformSnapshot(currentUser);

            song.Voices.Remove(songVoice);
            song.SetUpdated(currentUser.Id);
            await _songRepository.UpdateAsync(cancellationToken);

            //await _voiceRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}
