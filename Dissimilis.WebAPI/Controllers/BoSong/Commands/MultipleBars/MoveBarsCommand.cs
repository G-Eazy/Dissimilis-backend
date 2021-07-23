using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars
{
    public class MoveBarsCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public MoveBarDto Command { get; }


        public MoveBarsCommand(int songId, MoveBarDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class MoveBarsCommandHandler : IRequestHandler<MoveBarsCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public MoveBarsCommandHandler(SongRepository songRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(MoveBarsCommand request, CancellationToken cancellationToken)
        {
            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);

            song.MoveBars(request.Command.FromPosition, request.Command.MoveLength, request.Command.ToPostition);

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);

            await _songRepository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}