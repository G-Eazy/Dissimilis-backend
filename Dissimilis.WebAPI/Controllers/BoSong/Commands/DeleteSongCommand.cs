using Dissimilis.WebAPI.Controllers.BoSong.DTOs;
using Dissimilis.WebAPI.Controllers.SuperDTOs;
using Dissimilis.WebAPI.Database;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class DeleteSongCommand : MediatR.IRequest<SuperDTO>
    {
        public SuperDTO DeleteSongObject { get; private set; }

        public DeleteSongCommand(SuperDTO DeleteSongObject) 
        {
            this.DeleteSongObject = DeleteSongObject;
        }

    }
    public class DeleteSongCommandHandler : MediatR.IRequestHandler<DeleteSongCommand, SuperDTO>
    {
        private SongRepository _repository;
        public DeleteSongCommandHandler(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
        }
        public async Task<SuperDTO> Handle(DeleteSongCommand request, CancellationToken cancellationToken)
        {
            var SongModel = await _repository.DeleteSong(request, cancellationToken);
            SuperDTO SuperDTO = null;
            if (SongModel != null)
            {
                SuperDTO = new SuperDTO(SongModel.Id);
            }
            return SuperDTO;
        }


    }
}