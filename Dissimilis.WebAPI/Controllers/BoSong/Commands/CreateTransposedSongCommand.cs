using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class CreateTransposedSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public TransposeSongDto Command { get; }

        public CreateTransposedSongCommand(int songId, TransposeSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class CreateTransposedSongCommandHandler : IRequestHandler<CreateTransposedSongCommand, UpdatedSongCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _authService;

        public CreateTransposedSongCommandHandler(Repository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedSongCommandDto> Handle(CreateTransposedSongCommand request, CancellationToken cancellationToken)
        {
            var duplicateFromSong = await _repository.GetFullSongById(request.SongId, cancellationToken);

            var duplicatedSong = duplicateFromSong.Transpose(request.Command.Transpose);

            duplicatedSong.SetUpdated(_authService.GetVerifiedCurrentUser().Id);

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(duplicatedSong);
        }
    }
}
