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
        private readonly SongRepository _songRepository;
        private readonly NoteRepository _noteRepository;
        private readonly BarRepository _barRepository;
        private readonly IAuthService _IAuthService;

        public DeleteSongNoteCommandHandler(SongRepository songRepository, NoteRepository noteRepository, BarRepository barRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _noteRepository = noteRepository;
            _barRepository = barRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedCommandDto> Handle(DeleteSongNoteCommand request, CancellationToken cancellationToken)
        {
            var bar = await _barRepository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

            var songNote = bar.Notes.FirstOrDefault(songNote => songNote.Id == request.SongChordId);
            var song = await _songRepository.GetSongById(request.SongId, cancellationToken);
            var part = await _barRepository.GetSongBarById(request.SongId, request.SongVoiceId, request.SongBarId, cancellationToken);

            if (songNote == null)
            {
                throw new NotFoundException($"Chord with Id {request.SongChordId} not found");
            }
            song.PerformSnapshot(_IAuthService.GetVerifiedCurrentUser());

            bar.Notes.Remove(songNote);

            bar.SongVoice.SetSongVoiceUpdated(_IAuthService.GetVerifiedCurrentUser().Id);

            await _noteRepository.UpdateAsync(cancellationToken);

            return new UpdatedCommandDto(songNote);
        }
    }
}
