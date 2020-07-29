using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using Dissimilis.WebAPI.Database.Interfaces;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// This is the whole songs, which contains one or more parts 
    /// </summary>
    public class Song : BaseEntity, ISong
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
        /// ID of the creator, eg. instructor
        /// </summary>
        public User Arranger { get; set; }

        /// <summary>
        /// The id of the corresponding creator aka. User.Id
        /// </summary>
        public int ArrangerId { get; set; }

        /// <summary>
        /// The composer of the song
        /// </summary>
        [MaxLength(100)]
        public string Composer { get; set; }

        /// <summary>
        /// The time signature of the song (NO: Taktart)
        /// </summary>
        [MaxLength(10)]
        public string TimeSignature { get; set; }

        /// <summary>
        /// Empty constructor for Song
        /// </summary>
        public Song() { }

    }
}
