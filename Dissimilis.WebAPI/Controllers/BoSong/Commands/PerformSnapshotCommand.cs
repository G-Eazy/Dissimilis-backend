using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Services;
using Dissimilis.DbContext.Interfaces;
using Dissimilis.WebAPI.Extensions.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class PerformSnapshotCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; set; }

        public PerformSnapshotCommand(int songId)
        {
            SongId = songId;
        }
    }

    public class PerformSnapshotCommandHandler : IRequestHandler<PerformSnapshotCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _repository;
        private readonly IAuthService _IAuthService;

        public PerformSnapshotCommandHandler(SongRepository songRepository, IAuthService IAuthService)
        {
            _repository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(PerformSnapshotCommand request, CancellationToken cancellationToken)
        {
            var songToSnapshot = await _repository.GetFullSongById(request.SongId, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            songToSnapshot.PerformSnapshot(currentUser);
            //await _repository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(songToSnapshot);
        }
    }
}
