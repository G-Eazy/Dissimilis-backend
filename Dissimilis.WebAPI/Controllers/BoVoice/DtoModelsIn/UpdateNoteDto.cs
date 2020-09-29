using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.DTOs
{
    public class UpdateNoteDto
    {
        public int NoteNumber { get; set; }
        public int Length { get; set; }

        [MinLength(1)]
        [MaxLength(100)]
        public string[] Notes { get; set; } = new string[0];

    }
}
