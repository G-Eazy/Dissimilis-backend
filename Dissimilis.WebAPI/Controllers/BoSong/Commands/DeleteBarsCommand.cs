using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
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
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public DeleteBarsCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(DeleteBarsCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _repository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var song = await _repository.GetFullSongById(request.SongId, cancellationToken);

            song.DeleteBars(request.Command.FromPosition, request.Command.DeleteLength);

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(song, currentUser, cancellationToken);

            try
            {
                await _repository.UpdateAsync(song, currentUser, cancellationToken);
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
