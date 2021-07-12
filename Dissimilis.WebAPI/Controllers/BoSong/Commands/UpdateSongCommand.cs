using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;
using Dissimilis.WebAPI.Extensions.Models;


namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class UpdateSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public UpdateSongDto Command { get; }


        public UpdateSongCommand(int songId, UpdateSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class UpdateSongCommandHandler : IRequestHandler<UpdateSongCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public UpdateSongCommandHandler(SongRepository songRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(UpdateSongCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            song.PerformSnapshot(_IAuthService.GetVerifiedCurrentUser());

            song.Title = request.Command?.Title ?? song.Title;
            song.Composer = request.Command?.Composer ?? song.Composer;
            song.SongNotes = request.Command?.SongNotes ?? song.SongNotes;
            song.Speed = request.Command?.Speed ?? song.Speed;
            song.DegreeOfDifficulty = request.Command?.DegreeOfDifficulty ?? song.DegreeOfDifficulty;
            song.SetUpdated(_IAuthService.GetVerifiedCurrentUser());

            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}