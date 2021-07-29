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
        private readonly SongRepository _repository;
        private readonly BarRepository _barRepository;
        private readonly IAuthService _IAuthService;

        public UndoCommandHandler(SongRepository songRepository, BarRepository barRepository, IAuthService IAuthService)
        {
            _repository = songRepository;
            _barRepository = barRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(UndoCommand request, CancellationToken cancellationToken)
        {
            Song undoFromSong = await _repository.GetFullSongById(request.SongId, cancellationToken);
            undoFromSong.Undo();
            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(undoFromSong);
        }
    }
}
