using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class UpdateSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        public CreateSongVoiceDto Command { get; }

        public UpdateSongVoiceCommand(int songId, int songVoiceId, CreateSongVoiceDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class UpdateSongVoiceCommandHandler : IRequestHandler<UpdateSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly VoiceRepository _voiceRepository;
        private readonly IAuthService _IAuthService;

        public UpdateSongVoiceCommandHandler(SongRepository songRepository, VoiceRepository voiceRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _voiceRepository = voiceRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var songVoice = await _voiceRepository.GetSongVoiceById(request.SongId, request.SongVoiceId, cancellationToken);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
            song.PerformSnapshot(_IAuthService.GetVerifiedCurrentUser());

            songVoice.VoiceNumber = request.Command?.VoiceNumber ?? songVoice.VoiceNumber;
            songVoice.VoiceName = request.Command?.VoiceName ?? songVoice.VoiceName;
            songVoice.SetSongVoiceUpdated(_IAuthService.GetVerifiedCurrentUser().Id);
            
            await _voiceRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}