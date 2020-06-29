using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Song
    {
        [Key]
        public int SongId { get; set; }

        /// <summary>
        /// The song title of the music scheet (NO: Partitur)
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// ID of the creator, eg. instructor
        /// </summary>
        [Required]
        public User Creator { get; set; }
        public int CreatorId { get; set; }

        /// <summary>
        /// The composer of the song
        /// </summary>
        public string Composer { get; set; }

        /// <summary>
        /// The time signature of the song (NO: Taktart)
        /// </summary>
        public string TimeSignature { get; set; }

        /// <summary>
        /// The collection of parts found in a song
        /// </summary>
        public ICollection<Part> Parts { get; set; }

        /// <summary>
        /// Time and date of creation of voice
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? CreationTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedLast { get; set; }

        public Song() { }

        public Song(string title, User creator,
                    string composer, string TS, DateTime datetime)
        {
            this.Title = title;
            this.Creator = creator;
            this.Composer = composer;
            this.TimeSignature = TS;
            this.CreationTime = datetime;
        }
    }
}
