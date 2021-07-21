using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars
{
    public class DeleteBarsCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public DeleteBarDto Command { get; }

        public DeleteBarsCommand(int songId, DeleteBarDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class DeleteBarsCommandHandler : IRequestHandler<DeleteBarsCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public DeleteBarsCommandHandler(SongRepository songRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(DeleteBarsCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            song.DeleteBars(request.Command.FromPosition, request.Command.DeleteLength);

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);

            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}
