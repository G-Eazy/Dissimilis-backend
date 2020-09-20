using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class BarDto 
    {
        public int BarId { get; set; }
        public int BarNumber { get; set; }
        public bool RepBefore { get; set; }
        public bool RepAfter { get; set; }
        public int? House { get; set; }
        public NoteDto[] ChordsAndNotes { get; set; }

        public BarDto(Bar bar)
        {

            BarId = bar.Id;
            BarNumber = bar.BarNumber;
            RepBefore = bar.RepBefore;
            RepAfter = bar.RepAfter;
            House = bar.House;
            ChordsAndNotes = bar.Notes
                .OrderBy(n => n.NoteNumber)
                .Select(n => new NoteDto(n))
                .ToArray();
        }
    }
}
