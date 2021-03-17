using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Extensions.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class BarDto
    {
        public int BarId { get; set; }
        public int SongVoiceId { get; set; }
        public int SongId { get; set; }
        public int Position { get; set; }
        public bool RepBefore { get; set; }
        public bool RepAfter { get; set; }
        public int? House { get; set; }
        public NoteDto[] Chords { get; set; }

        public BarDto(SongBar songBar)
        {
            BarId = songBar.Id;
            SongVoiceId = songBar.SongVoiceId;
            SongId = songBar.SongVoice.SongId;

            Position = songBar.Position;
            RepBefore = songBar.RepBefore;
            RepAfter = songBar.RepAfter;
            House = songBar.House;
            Chords = songBar.GetBarNotes()
                .OrderBy(n => n.Postition)
                .Select(n => new NoteDto(n))
                .ToArray();
        }
    }
}
