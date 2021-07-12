using System;
using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut;
using Dissimilis.DbContext.Models;
using Newtonsoft.Json.Linq;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut
{
    public class SongVoiceDto
    {
        public int SongVoiceId { get; set; }
        public int SongId { get; set; }
        public string Instrument { get; set; }
        public string VoiceName { get; set; }
        public bool IsMain { get; set; }
        public int PartNumber { get; set; }
        public BarDto[] Bars { get; set; }


        public SongVoiceDto(SongVoice songVoice)
        {
            SongVoiceId = songVoice.Id;
            SongId = songVoice.SongId;
            PartNumber = songVoice.VoiceNumber;
            VoiceName = songVoice.VoiceName ?? songVoice.Instrument?.Name;
            IsMain = songVoice.IsMainVoice;
            Instrument = songVoice.Instrument?.Name;
            Bars = songVoice.SongBars
                .OrderBy(b => b.Position)
                .Select(b => new BarDto(b))
                .ToArray();

        }

        public SongVoiceDto () { }

        public static SongVoiceDto JsonToSongVoiceDto(JToken json)
        {
            return new SongVoiceDto()
            {
                SongVoiceId = json["SongVoiceId"].Value<int>(),
                SongId = json["SongId"].Value<int>(),
                PartNumber = json["PartNumber"].Value<int>(),
                VoiceName = json["VoiceName"].Value<string>(),
                IsMain = json["IsMain"].Value<bool>(),
                Instrument = json["Instrument"].Value<string>(),
                Bars = null,
            };
        }

        public static SongVoice ConvertToSongVoice( SongVoiceDto voiceDto, DateTimeOffset updatedOn, int updatedById)
        {
            return new SongVoice()
            {
                Id = voiceDto.SongVoiceId,
                VoiceName = voiceDto.VoiceName,
                VoiceNumber = voiceDto.PartNumber,
                IsMainVoice = voiceDto.IsMain,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                SongId = voiceDto.SongId
            };
        }
    }
}