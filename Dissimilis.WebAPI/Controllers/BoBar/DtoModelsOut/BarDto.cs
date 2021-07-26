using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;

namespace Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut
{
    public class BarDto
    {
        public int BarId { get; set; }
        public int SongVoiceId { get; set; }
        public int SongId { get; set; }
        public int Position { get; set; }
        public bool RepBefore { get; set; }
        public bool RepAfter { get; set; }
        public int? VoltaBracket { get; set; }
        public NoteDto[] Chords { get; set; }

        public BarDto(SongBar songBar)
        {
            BarId = songBar.Id;
            SongVoiceId = songBar.SongVoiceId;
            SongId = songBar.SongVoice.SongId;

            Position = songBar.Position;
            RepBefore = songBar.RepBefore;
            RepAfter = songBar.RepAfter;
            VoltaBracket = songBar.VoltaBracket;
            Chords = songBar.GetBarNotes()
                .OrderBy(n => n.Position)
                .Select(n => new NoteDto(n))
                .ToArray();
        }
    }
}
