using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Services;
using Dissimilis.WebAPI.Extensions.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.DbContext.Models.Song;
using System.Collections.Generic;
using Dissimilis.WebAPI.Controllers.BoNote;
using Dissimilis.WebAPI.Controllers.BoBar;
using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class UndoCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; set; }
        
        public UndoCommand(int songId)
        {
            SongId = songId;
        }
    }

    public class UndoCommandHandler : IRequestHandler<UndoCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _repository;
        private readonly BarRepository _barRepository;
        private readonly IAuthService _IAuthService;

        public UndoCommandHandler(SongRepository songRepository, BarRepository barRepository, IAuthService IAuthService)
        {
            _repository = songRepository;
            _barRepository = barRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(UndoCommand request, CancellationToken cancellationToken)
        {
            Song undoFromSong = await _repository.GetFullSongById(request.SongId, cancellationToken);
            SongSnapshot snapshot = undoFromSong.PopSnapshot(true);
            SongByIdDto deserialisedSong = Newtonsoft.Json.JsonConvert.DeserializeObject<SongByIdDto>(snapshot.SongObjectJSON);
            User currentUser = _IAuthService.GetVerifiedCurrentUser();

            undoFromSong.Undo();
            await _repository.UpdateAsync(cancellationToken);

            /*undoFromSong.RemoveElementsFromOldSong(deserialisedSong.Voices);
            await _repository.SaveAsync(undoFromSong, cancellationToken);
            await _repository.UpdateAsync(cancellationToken);

            undoFromSong.AddSnapshotValues(deserialisedSong, currentUser);
            await _repository.SaveAsync(undoFromSong, cancellationToken);
            await _repository.UpdateAsync(cancellationToken);*/


            /*Song undoFromSong;
            //await using (var transaction = await _repository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)) {
                //undoFromSong.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);
                undoFromSong = await _repository.GetFullSongById(request.SongId, cancellationToken);

                var currentUser = _IAuthService.GetVerifiedCurrentUser();

                Song undoneSong;
                List<SongNote> removeNotes;

                (undoneSong, removeNotes) = undoFromSong.GetUndoneSong();

                foreach (var note in removeNotes)
                {
                    Console.WriteLine(note.NoteValues);
                    var bar = await _barRepository.GetSongBarById(request.SongId, note.SongBar.SongVoiceId, note.BarId, cancellationToken);
                    bar.Notes.Remove(note);
                }

                await _repository.UpdateAsync(cancellationToken);

                undoFromSong.Title = undoneSong.Title;
                undoFromSong.UpdatedBy = undoneSong.CreatedBy;
                undoFromSong.UpdatedOn = DateTimeOffset.Now;
                undoFromSong.Voices = undoneSong.Voices;

                undoFromSong.SetUpdatedOverAll(_IAuthService.GetVerifiedCurrentUser().Id);
            await _repository.UpdateAsync(cancellationToken);*/

            /*try
            {
                await _repository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException(e.Message);
            }*/
            //}
            return new UpdatedSongCommandDto(undoFromSong);
        }
    }
}
