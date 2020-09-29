using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn
{
    public class UpdatedCommandDto
    {
        public int SongId { get; set; }
        public int SongVoiceId { get; set; }
        public int SongBarId { get; set; }
        public int SongNoteId { get; set; }

        public UpdatedCommandDto(SongVoice songVoice)
        {
            SongId = songVoice.SongId;
            SongVoiceId = songVoice.Id;
        }

        public UpdatedCommandDto(SongBar songBar)
        {
            SongBarId = songBar.Id;
        }

        public UpdatedCommandDto(SongNote songNote)
        {
            SongNoteId = songNote.Id;
        }

     
    }
}
