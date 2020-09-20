namespace Dissimilis.WebAPI.DTOs
{
    public class NewPartDTO 
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public byte PartNumber { get; set; }
        public NewBarDTO[] Bars { get; set; }

        public NewPartDTO() { }
    } 
}
