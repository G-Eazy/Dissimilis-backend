using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongVoiceDto
    {
        public int SongVoiceId { get; set; }
        public int SongId { get; set; }

        public string Title { get; set; }
        public bool IsMain { get; set; }
        public int PartNumber { get; set; }
        public BarDto[] Bars { get; set; }


        public SongVoiceDto(SongVoice songVoice)
        {
            SongVoiceId = songVoice.Id;
            SongId = songVoice.SongId;
            PartNumber = songVoice.VoiceNumber;
            Title = songVoice.Instrument?.Name ?? "Main";
            IsMain = songVoice.IsMainVoice;

            Bars = songVoice.SongBars
                .OrderBy(b => b.BarNumber)
                .Select(b => new BarDto(b))
                .ToArray();

        }
    }
}