using Dissimilis.WebAPI.Controllers.BoSong.DTOs;
using Dissimilis.WebAPI.Database;
using Dissimilis.WebAPI.Controllers.BoUser;
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

        public UserRepository _user_repository { get; set; }

        public CreateSongCommandHandler(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
            this._user_repository = new UserRepository(context);
        }
        public async Task<SongDTO> Handle(CreateSongCommand request, CancellationToken cancellationToken)
        {
            var ArrangerId = request.NewSongObject.ArrangerId;
            var ExistsArranger = await this._user_repository.GetUserById(ArrangerId, cancellationToken);
            SongDTO SongObject = null;
            if (ExistsArranger != null)
            { 
                var SongModel = await _repository.CreateSong(request, cancellationToken);
                SongObject = new SongDTO(SongModel);
            }


            return SongObject;
        }


    }
}