using System;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models.Song
{
    public class SongSnapshot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SongId { get; set; }

        public Song Song { get; set; }

        [Required]
        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        [Required]
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        
        [Required]
        public string SongObjectJSON { get; set; }

        public SongSnapshot() { }
    }
}
