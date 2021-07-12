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
            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            song.PerformSnapshot(currentUser);


            song.MoveBars(request.Command.FromPosition, request.Command.MoveLength, request.Command.ToPostition);

            song.SetUpdatedOverAll(currentUser.Id);

            await _songRepository.UpdateAsync(cancellationToken);

            try
            {
                await _songRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }

            return new UpdatedSongCommandDto(song);
        }
    }
}