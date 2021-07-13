using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class ShareSongUserCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public ShareSongDto Command { get; }


        public ShareSongUserCommand(int songId, ShareSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class ShareSongUserCommandHandler : IRequestHandler<ShareSongUserCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public ShareSongUserCommandHandler(SongRepository songRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(ShareSongUserCommand request, CancellationToken cancellationToken)
        {
            
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);


            return new UpdatedSongCommandDto(song);
        }
    }
}