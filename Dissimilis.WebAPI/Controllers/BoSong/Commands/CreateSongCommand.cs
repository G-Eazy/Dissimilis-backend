using Dissimilis.WebAPI.Controllers.BoSong.DTOs;
using Dissimilis.WebAPI.Database;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class CreateSongCommand : MediatR.IRequest<SongDTO>
    {
        public NewSongDTO NewSongObject { get; private set; }

        public CreateSongCommand(NewSongDTO NewSongObject) 
        {
            this.NewSongObject = NewSongObject;
        }

    }
    public class CreateSongCommandHandler : MediatR.IRequestHandler<CreateSongCommand, SongDTO>
    {
        private SongRepository _repository;
        public CreateSongCommandHandler(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
        }
        public async Task<SongDTO> Handle(CreateSongCommand request, CancellationToken cancellationToken)
        {
            var SongModel = await _repository.CreateSong(request, cancellationToken);
            var SongDTO = new SongDTO(SongModel);
            return SongDTO;
        }


    }
}