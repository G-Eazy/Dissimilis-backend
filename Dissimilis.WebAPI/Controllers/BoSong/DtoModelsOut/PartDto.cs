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


        public PartDto(Part part)
        {
            if (part?.Instrument == null)
                return;

            PartId = part.Id;
            PartNumber = part.PartNumber;
            Title = part.Instrument.Name;
            Bars = part.Bars
                .OrderBy(b => b.BarNumber)
                .Select(b => new BarDto(b))
                .ToArray();

        }
    }
}