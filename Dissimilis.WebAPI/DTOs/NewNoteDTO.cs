namespace Dissimilis.WebAPI.DTOs
{
    public class NewNoteDTO
    {
        public int BarId { get; set; }
        public byte NoteNumber { get; set; }
        public byte Length { get; set; }
        public string[] Notes { get; set; }

        public NewNoteDTO() { }

    }
}
