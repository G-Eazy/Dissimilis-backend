using System.Collections.Generic;
using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Newtonsoft.Json.Linq;

namespace Dissimilis.WebAPI.Controllers.BoBar
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

        public BarDto() { }

        public static BarDto JsonToBarDto(JToken json)
        {
            int voltaBracket = (json["VoltaBracket"].Value<string>() != null) ? int.Parse(json["VoltaBracket"].Value<string>()) : 0;
            return new BarDto()
            {
                BarId = int.Parse(json["BarId"].Value<string>()),
                SongVoiceId = int.Parse(json["SongVoiceId"].Value<string>()),
                SongId = int.Parse(json["SongId"].Value<string>()),
                Position = int.Parse(json["Position"].Value<string>()),
                RepBefore = bool.Parse(json["RepBefore"].Value<string>()),
                RepAfter = bool.Parse(json["RepAfter"].Value<string>()),
                VoltaBracket = (voltaBracket != 0) ? voltaBracket : null,
                Chords = null
            };
        }

        public static SongBar ConvertToSongBar(BarDto barDto, SongVoice voice)
        {
            return new SongBar()
            {
                Position = barDto.Position,
                RepBefore = barDto.RepBefore,
                RepAfter = barDto.RepAfter,
                VoltaBracket = barDto.VoltaBracket,
                SongVoiceId = barDto.SongVoiceId,
                Notes = new List<SongNote>(),
                SongVoice = voice
            };
        }
    }
}
