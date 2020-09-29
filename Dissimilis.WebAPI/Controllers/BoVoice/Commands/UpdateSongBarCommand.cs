using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class UpdateSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        public int BarId { get; }
        public UpdateBarDto Command { get; }

        public UpdateSongBarCommand(int songId, int songVoiceId, int barId, UpdateBarDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            BarId = barId;
            Command = command;
        }
    }

    public class UpdateSongBarCommandHandler : IRequestHandler<UpdateSongBarCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly AuthService _authService;

        public UpdateSongBarCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongBarCommand request, CancellationToken cancellationToken)
        {
            var bar = await _repository.GetSongBarById(request.SongId, request.SongVoiceId, request.BarId, cancellationToken);

            bar.RepAfter = request.Command.RepAfter;
            bar.RepBefore = request.Command.RepBefore;
            bar.House = request.Command.House;


            bar.SongVoice.SetSongVoiceUpdated(_authService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(bar);
        }
    }
}