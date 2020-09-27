using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class PartDto
    {
        public int PartId { get; set; }

        /// <summary>
        /// Instrument name
        /// </summary>
        public string Title { get; set; }
        public int PartNumber { get; set; }
        public BarDto[] Bars { get; set; }


        public PartDto(SongVoice songVoice)
        {
            if (songVoice?.Instrument == null)
                return;

            PartId = songVoice.Id;
            PartNumber = songVoice.PartNumber;
            Title = songVoice.Instrument.Name;
            Bars = songVoice.Bars
                .OrderBy(b => b.BarNumber)
                .Select(b => new BarDto(b))
                .ToArray();

        }
    }
}