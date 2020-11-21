﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class UpdateSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        public UpdateSongVoiceDto Command { get; }

        public UpdateSongVoiceCommand(int songId, int songVoiceId, UpdateSongVoiceDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            Command = command;
        }
    }

    public class UpdateSongVoiceCommandHandler : IRequestHandler<UpdateSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public UpdateSongVoiceCommandHandler(Repository repository, IAuthService IAuthService)
        {
            _repository = repository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(UpdateSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var song = await _repository.GetSongById(request.SongId, cancellationToken);
            var songVoice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);

            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with id {request.SongVoiceId} not found");
            }

            if (songVoice.Instrument?.Name != request.Command.Instrument)
            {
                var instrument = await _repository.CreateOrFindInstrument(request.Command.Instrument, cancellationToken);
                songVoice.Instrument = instrument;
            }

            songVoice.VoiceNumber = request.Command.VoiceNumber;

            songVoice.SetSongVoiceUpdated(_IAuthService.GetVerifiedCurrentUser().Id);
            
            await _repository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songVoice);
        }
    }
}