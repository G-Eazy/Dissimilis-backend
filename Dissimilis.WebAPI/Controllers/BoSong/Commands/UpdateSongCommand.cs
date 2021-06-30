using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class UpdateSongCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public UpdateSongDto Command { get; }


        public UpdateSongCommand(int songId, UpdateSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class UpdateSongCommandHandler : IRequestHandler<UpdateSongCommand, UpdatedSongCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public UpdateSongCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(UpdateSongCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if (!String.IsNullOrEmpty(request.Command.Title))
            {
            song.Title = request.Command.Title;
            }
            if ( request.Command.Composer != null)
            {
                song.Composer = request.Command.Composer;
            }
            if (request.Command.SongNotes != null)
            {
                song.SongNotes = request.Command.SongNotes;
            }
            if (request.Command.Speed < 0)
            {
                throw new Exception("Song speed needs to be a positive number");
            }
            else if (request.Command.Speed != null)
            {
                song.Speed = request.Command.Speed;
            }
           if ( request.Command.DegreeOfDifficulty < 1 || request.Command.DegreeOfDifficulty > 10)
            {
                throw new Exception("Degree of difficulty needs to be a number between and inclusive 1 and 10");
            }
            else if ( request.Command.DegreeOfDifficulty != null)
            {
                song.DegreeOfDifficulty = request.Command.DegreeOfDifficulty;
            }
                song.SetUpdated(_IAuthService.GetVerifiedCurrentUser());

            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedSongCommandDto(song);
        }
    }
}