using System;
using System.Collections.Generic;
using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Extensions.Models;
using Newtonsoft.Json.Linq;

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
                .OrderBy(n => n.Position)
                .Select(n => new NoteDto(n))
                .ToArray();
        }

        public BarDto() { }

        public static BarDto JsonToBarDto(JToken json)
        {
            int house = (json["House"].Value<string>() != null) ? int.Parse(json["House"].Value<string>()) : 0;
            return new BarDto()
            {
                BarId = int.Parse(json["BarId"].Value<string>()),
                SongVoiceId = int.Parse(json["SongVoiceId"].Value<string>()),
                SongId = int.Parse(json["SongId"].Value<string>()),
                Position = int.Parse(json["Position"].Value<string>()),
                RepBefore = bool.Parse(json["RepBefore"].Value<string>()),
                RepAfter = bool.Parse(json["RepAfter"].Value<string>()),
                House = (house != 0) ? house : null,
                Chords = null
            };
        }

        public static SongBar ConvertToSongBar(BarDto barDto)
        {
            return new SongBar()
            {
                Id = barDto.BarId,
                Position = barDto.Position,
                RepBefore = barDto.RepBefore,
                RepAfter = barDto.RepAfter,
                House = barDto.House,
                SongVoiceId = barDto.SongVoiceId,
                Notes = new List<SongNote>()
            };
        }
    }
}
