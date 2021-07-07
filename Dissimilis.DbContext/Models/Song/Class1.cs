using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Interfaces;

namespace Dissimilis.DbContext.Models.Song
{
    class UndoStack
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SongId { get; set; }
        
        [Required]
        public Song song { get; set; }
        
        [Required]
        public string SongObjectJSON { get; set; }
    }
}
