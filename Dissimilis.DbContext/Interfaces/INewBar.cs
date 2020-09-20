namespace Dissimilis.DbContext.Interfaces
{
    public interface INewBar
    {
        public ushort BarNumber { get; }
        public int PartId { get; }
        public bool RepBefore { get; }
        public bool RepAfter { get; }
        public byte? House { get; }
        public NewNoteDTO[] ChordsAndNotes { get; }
    }
}
