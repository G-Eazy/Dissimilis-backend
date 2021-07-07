using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class CopyBarsCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public CopyBarDto Command { get; }


        public CopyBarsCommand(int songId, CopyBarDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class CopyBarsCommandHandler : IRequestHandler<CopyBarsCommand, UpdatedSongCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public CopyBarsCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(CopyBarsCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _repository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var song = await _repository.GetFullSongById(request.SongId, cancellationToken);

            song.CopyBars(request.Command.FromPosition, request.Command.CopyLength, request.Command.ToPosition);

            song.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(song, user, cancellationToken);

            try
            {
                await _repository.UpdateAsync(song, user, cancellationToken);
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