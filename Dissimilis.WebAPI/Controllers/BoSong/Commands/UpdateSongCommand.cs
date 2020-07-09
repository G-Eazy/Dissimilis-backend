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
    public class UpdateSongCommand : MediatR.IRequest<SuperDTO>
    {
        public SuperDTO UpdateSongObject { get; private set; }

        public UpdateSongCommand(SuperDTO UpdateSongObject) 
        {
            this.UpdateSongObject = UpdateSongObject;
        }

    }
    public class UpdateSongCommandHandler : MediatR.IRequestHandler<UpdateSongCommand, SuperDTO>
    {
        private SongRepository _repository;
        public UpdateSongCommandHandler(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
        }
        public async Task<SuperDTO> Handle(UpdateSongCommand request, CancellationToken cancellationToken)
        {
            var SongModel = await _repository.UpdateSong(request, cancellationToken);
            SuperDTO SuperDTO = null;
            if (SongModel != null)
            {
                SuperDTO = new SuperDTO(SongModel.Id);
            }
            return SuperDTO;
        }


    }
}