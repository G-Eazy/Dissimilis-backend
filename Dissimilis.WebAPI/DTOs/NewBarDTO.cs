using Dissimilis.DbContext.Interfaces;

namespace Dissimilis.WebAPI.DTOs
{
    public class NewBarDTO : INewBar
    {
        public ushort BarNumber { get; set; }
        public int PartId { get; set; }
        public bool RepBefore { get; set; } 
        public bool RepAfter { get; set; } 
        public byte? House { get; set; }
        public NewNoteDTO[] ChordsAndNotes { get; set; }

    }
}
