using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Extensions.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class UndoCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; set; }
        
        public UndoCommand(int songId)
        {
            SongId = songId;
        }
    }

    public class UndoCommandHandler : IRequestHandler<UndoCommand, UpdatedSongCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public UndoCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(UndoCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _repository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var undoFromSong = await _repository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var result = await undoFromSong.Undo(currentUser);
            undoFromSong.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);
            try
            {
                await _repository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            } catch(Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }
            return result;

        }
    }
}
