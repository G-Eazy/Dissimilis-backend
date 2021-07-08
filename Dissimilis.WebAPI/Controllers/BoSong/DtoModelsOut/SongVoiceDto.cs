using System;
using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
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
            foreach(var bar in songVoice.SongBars)
            {
                Console.WriteLine(bar.SongVoice);
            }
            Bars = songVoice.SongBars
                .OrderBy(b => b.Position)
                .Select(b => new BarDto(b))
                .ToArray();

        }

        public static SongVoice ConvertToSongVoice( SongVoiceDto voiceDto, DateTimeOffset updatedOn, User updatedBy, int updatedById, Song song)
        {
            return new SongVoice()
            {
                Id = voiceDto.SongVoiceId,
                VoiceName = voiceDto.VoiceName,
                VoiceNumber = voiceDto.PartNumber,
                IsMainVoice = voiceDto.IsMain,
                UpdatedBy = updatedBy,
                UpdatedById = updatedById,
                UpdatedOn = updatedOn,
                Song = song,
                SongId = song.Id
            };
        }
    }
}