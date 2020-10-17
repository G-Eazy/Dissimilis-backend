using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class CreateSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public CreateSongDto Command { get; }

        public CreateSongCommand(CreateSongDto command)
        {
            Command = command;
        }
    }

    public class CreateSongCommandHandler : IRequestHandler<CreateSongCommand, UpdatedSongCommandDto>
    {
        private readonly Repository _repository;
        private readonly AuthService _authService;

        public CreateSongCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(CreateSongCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();

            var song = new Song()
            {
                Title = request.Command.Title,
                Numerator = request.Command.Numerator,
                Denominator = request.Command.Denominator,
                Arranger = currentUser
            };

            song.SetCreated(currentUser);
            await _repository.SaveAsync(song, cancellationToken);

            var mainVoice = new SongVoice()
            {
                SongId = song.Id,
                VoiceNumber = 1,
                IsMainVoice = true,
                SongBars = new SongBar[] { new SongBar() { Position = 1 } }
            };

            song.Voices.Add(mainVoice);
            mainVoice.SetCreated(currentUser);

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}