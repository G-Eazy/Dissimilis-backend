using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Data.IsolationLevel;

namespace Dissimilis.WebAPI.Controllers.BoVoice
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
        private readonly Repository _repository;
        private readonly IAuthService _IAuthService;

        public CreateSongNoteCommandHandler(Repository repository, IAuthService authService)
        {
            _repository = repository;
            _IAuthService = authService;
    }

        public async Task<UpdatedCommandDto> Handle(CreateSongNoteCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            SongNote note;
            await using (var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
            {
                var song = await _repository.GetSongById(request.SongId, cancellationToken);
                song.PerformSnapshot(currentUser);

                var songBar = await _repository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);
                if (songBar.Notes.Any(n => n.Position == request.Command.Position))
                {
                    throw new ValidationException("Note number already in use");
                }

                note = new SongNote()
                {
                    Position = request.Command.Position,
                    Length = request.Command.Length,
                    ChordName = request.Command.ChordName
                };

                if (note.ChordName != null)
                {
                    note.SetNoteValues(SongNoteExtension.GetNoteValuesFromChordName(note.ChordName).ToArray());
                }
                else 
                {
                    note.SetNoteValues(request.Command.Notes);
                }
                songBar.Notes.Add(note);
                songBar.CheckSongBarValidation();
                songBar.SongVoice.SetSongVoiceUpdated(currentUser.Id);

                try
                {
                    await _repository.UpdateAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new ValidationException(e.Message);
                }
            }
            return new UpdatedCommandDto(note);
        }
    }
}