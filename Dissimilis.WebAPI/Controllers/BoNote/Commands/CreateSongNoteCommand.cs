﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoBar;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Data.IsolationLevel;

namespace Dissimilis.WebAPI.Controllers.BoNote.Commands
{
    public class CreateSongNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; }
        public CreateNoteDto Command { get; }

        public CreateSongNoteCommand(int songId, int songVoiceId, int songBarId, CreateNoteDto command)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            Command = command;
        }
    }

    public class CreateSongNoteCommandHandler : IRequestHandler<CreateSongNoteCommand, UpdatedCommandDto>
    {
        private readonly NoteRepository _noteRepository;
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public CreateSongNoteCommandHandler(NoteRepository noteRepository, BarRepository barRepository, SongRepository songRepository, IAuthService authService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _noteRepository = noteRepository;
            _barRepository = barRepository;
            _songRepository = songRepository;
            _IAuthService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedCommandDto> Handle(CreateSongNoteCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            SongNote songNote;

            await using (var transaction = await _noteRepository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
            {
                var songBar = await _barRepository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

                if (songBar.Notes.Any(n => n.Position == request.Command.Position))
                {
                    throw new ValidationException("Note number already in use");
                }

                songNote = new SongNote()
                {
                    Position = request.Command.Position,
                    Length = request.Command.Length,
                    ChordName = request.Command.ChordName
                };

                if (songNote.ChordName != null)
                {
                    songNote.SetNoteValues(SongNoteExtension.GetNoteValuesFromChordName(songNote.ChordName).ToArray());
                }
                else 
                {
                    songNote.SetNoteValues(request.Command.Notes);
                }
                songBar.Notes.Add(songNote);
                songBar.CheckSongBarValidation();
                songBar.SongVoice.SetSongVoiceUpdated(currentUser.Id);

                try
                {
                    await _noteRepository.UpdateAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new ValidationException("Transaction error, aborting operation. Please try again.");
                }
            }
            return new UpdatedCommandDto(songNote);
        }
    }
}