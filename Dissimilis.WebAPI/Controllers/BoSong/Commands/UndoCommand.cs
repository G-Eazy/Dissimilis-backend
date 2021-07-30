using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Services;
using Dissimilis.WebAPI.Extensions.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.DbContext.Models.Song;
using System.Collections.Generic;
using Dissimilis.WebAPI.Controllers.BoNote;
using Dissimilis.WebAPI.Controllers.BoBar;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;

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
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _permissionChecker;

        public UndoCommandHandler(SongRepository songRepository, IAuthService IAuthService, IPermissionCheckerService permissionChecker)
        {
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _permissionChecker = permissionChecker;
        }

        public async Task<UpdatedSongCommandDto> Handle(UndoCommand request, CancellationToken cancellationToken)
        {
            Song undoFromSong = await _songRepository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            if (!await _permissionChecker.CheckPermission(undoFromSong, currentUser, Operation.Modify, cancellationToken))
                throw new UnauthorizedAccessException("User is not allowed to modify this song.");

            using (var transaction = _songRepository.Context.Database.BeginTransaction())
            {
                try
                {
                    undoFromSong.Undo();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            await _songRepository.UpdateAsync(cancellationToken);


            return new UpdatedSongCommandDto(undoFromSong);
        }
    }
}
