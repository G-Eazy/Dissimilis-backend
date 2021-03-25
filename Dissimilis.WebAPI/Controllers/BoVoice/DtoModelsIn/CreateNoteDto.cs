using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.DTOs
{
    public class CreateNoteDto
    {
        [Range(0,int.MaxValue)]
        public int Position { get; set; }

        [Range(1,int.MaxValue)]
        public int Length { get; set; }


        [MinLength(1)]
        [MaxLength(100)]
        public string[] Notes { get; set; } = new string[0];

        [MaxLength(10)]
        public string ActiveChord { get; set; }
    }
}
