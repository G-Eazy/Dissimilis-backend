using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoVoice.Commands
{
    public class DeleteSongVoiceCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; }
        //public IMediator _mediator { get; set; }

        public DeleteSongVoiceCommand(int songId, int songVoiceId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
        }
    }

    public class DeleteSongVoiceCommandHandle : IRequestHandler<DeleteSongVoiceCommand, UpdatedCommandDto>
    {
        private readonly VoiceRepository _voiceRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;

        public DeleteSongVoiceCommandHandle(VoiceRepository voiceRepository, SongRepository songRepository, IAuthService IAuthService)
        {
            _voiceRepository = voiceRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongVoiceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            await using var transaction = await _voiceRepository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var song = await _songRepository.GetFullSongById(request.SongId, cancellationToken);

            var songVoice = song.Voices.SingleOrDefault(sv => sv.Id == request.SongVoiceId);
            if (songVoice == null)
            {
                throw new NotFoundException($"Voice with Id {request.SongVoiceId} not found");
            }
            song.PerformSnapshot(currentUser);

            song.Voices.Remove(songVoice);
            song.SetUpdatedOverAll(currentUser.Id);
            await _voiceRepository.UpdateAsync(cancellationToken);
            try
            {
                await _voiceRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }
            //await _songRepository.UpdateAsync(cancellationToken);


            return new UpdatedCommandDto(songVoice);
        }
    }
}
