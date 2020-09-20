using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Interfaces;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// This is the whole songs, which contains one or more parts 
    /// </summary>
    public class Song : ICreatedAndUpdated
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The song title of the music scheet (NO: Partitur)
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }


        /// <summary>
        /// The composer of the song
        /// </summary>
        [MaxLength(100)]
        public string Composer { get; set; }

        /// <summary>
        /// The time signature of the song (NO: Taktart)
        /// </summary>
        [Range(-10, 10)]
        public int Numerator { get; set; }
        [Range(1, 10)]
        public int Denominator { get; set; }



        /// <summary>
        /// ID of the creator, eg. instructor
        /// </summary>
        public User Arranger { get; set; }
        public int ArrangerId { get; set; }

        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }

        public User UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }

        public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}
