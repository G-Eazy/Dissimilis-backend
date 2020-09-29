﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Data.IsolationLevel;

namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class DeleteSongBarCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int BarId { get; }

        public DeleteSongBarCommand(int songId, int songVoiceId, int barId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            BarId = barId;
        }
    }

    public class DeleteSongBarCommandHandler : IRequestHandler<DeleteSongBarCommand, UpdatedCommandDto>
    {
        private readonly Repository _repository;
        private readonly AuthService _authService;

        public DeleteSongBarCommandHandler(Repository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongBarCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            await using (var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
            {
                var song = await _repository.GetSongById(request.SongId, cancellationToken);

                var songVoice = song.Voices.FirstOrDefault(v => v.Id == request.SongVoiceId);
                if (songVoice == null)
                {
                    throw new NotFoundException($"Voice with Id {request.SongVoiceId} was not found");
                }

                var bar = songVoice.SongBars.FirstOrDefault(b => b.Id == request.BarId);
                if (bar == null)
                {
                    throw new NotFoundException($"Bar with Id {request.BarId} was not found");
                }

                song.RemoveSongBarFromAllVoices(bar.BarNumber);

                song.UpdateAllSongVoices(currentUser.Id);

                try
                {
                    await _repository.UpdateAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new ValidationException("Transaction error happend, aborting operation. Please try again.");
                }
            }

            return null;
        }
    }
}