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
        public int VoiceNumber { get; set; }
        public BarDto[] Bars { get; set; }


        public SongVoiceDto(SongVoice songVoice)
        {
            SongVoiceId = songVoice.Id;
            SongId = songVoice.SongId;
            VoiceNumber = songVoice.VoiceNumber;
            VoiceName = songVoice.VoiceName ?? songVoice.Instrument?.Name;
            IsMain = songVoice.IsMainVoice;
            Instrument = songVoice.Instrument?.Name;
            Bars = songVoice.SongBars
                .OrderBy(b => b.Position)
                .Select(b => new BarDto(b))
                .ToArray();

        }

        public SongVoiceDto () { }

        public static SongVoice ConvertToSongVoice(SongVoiceDto voiceDto, DateTimeOffset updatedOn, int updatedById, Song song)
        {
            SongVoice voice = new SongVoice() 
            {
                SongId = voiceDto.SongId,
                VoiceNumber = voiceDto.VoiceNumber,
                VoiceName = voiceDto.VoiceName,
                Song = song,
                IsMainVoice = voiceDto.IsMain,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn
            };

            return voice;
        }
    }
}