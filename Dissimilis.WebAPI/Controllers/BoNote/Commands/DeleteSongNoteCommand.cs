using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dissimilis.WebAPI.Controllers.BoBar;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using Dissimilis.WebAPI.Controllers.BoSong;
using System;
using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.Controllers.BoNote.Commands
{
    public class DeleteSongNoteCommand : IRequest<UpdatedCommandDto>
    {
        public int SongId { get; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; }
        public int SongChordId { get; }

        public DeleteSongNoteCommand(int songId, int songVoiceId, int songBarId, int songChordId)
        {
            SongId = songId;
            SongVoiceId = songVoiceId;
            SongBarId = songBarId;
            SongChordId = songChordId;
        }
    }

    public class DeleteSongNoteCommandHandler : IRequestHandler<DeleteSongNoteCommand, UpdatedCommandDto>
    {
        private readonly NoteRepository _noteRepository;
        private readonly BarRepository _barRepository;
        private readonly SongRepository _songRepository;
        private readonly IAuthService _IAuthService;
        private readonly _IPermissionCheckerService _IPermissionCheckerService;

        public DeleteSongNoteCommandHandler(NoteRepository noteRepository, BarRepository barRepository, SongRepository songRepository, IAuthService IAuthService, _IPermissionCheckerService IPermissionCheckerService)
        {
            _noteRepository = noteRepository;
            _barRepository = barRepository;
            _songRepository = songRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongNoteCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();

            var bar = await _barRepository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

            var songNote = bar.Notes.FirstOrDefault(songNote => songNote.Id == request.SongChordId);

            if (songNote == null)
            {
                throw new NotFoundException($"Chord with Id {request.SongChordId} not found");
            }

            bar.Notes.Remove(songNote);

            bar.SongVoice.SetSongVoiceUpdated(_IAuthService.GetVerifiedCurrentUser().Id);

            await _noteRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songNote);
        }
    }
}
