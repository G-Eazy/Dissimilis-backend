using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongVoiceDto
    {
        public int PartId { get; set; }

        /// <summary>
        /// Instrument name
        /// </summary>
        public string Title { get; set; }
        public int PartNumber { get; set; }
        public BarDto[] Bars { get; set; }


        public SongVoiceDto(SongVoice songVoice)
        {
            PartId = songVoice.Id;
            PartNumber = songVoice.VoiceNumber;
            Title = songVoice.Instrument?.Name;
            Bars = songVoice.SongBars
                .OrderBy(b => b.BarNumber)
                .Select(b => new BarDto(b))
                .ToArray();

        }
    }
}