using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.WebAPI.Database.Models
{
    public class Song : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The song title of the music scheet (NO: Partitur)
        /// </summary>
        [Required]
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
        public string Composer { get; set; }

        /// <summary>
        /// The time signature of the song (NO: Taktart)
        /// </summary>
        public string TimeSignature { get; set; }

        /// <summary>
        /// Empty constructor for Song
        /// </summary>
        public Song() { }

        /// <summary>
        /// COnstructor for Song
        /// </summary>
        /// <param name="title"></param>
        /// <param name="creator"></param>
        /// <param name="composer"></param>
        /// <param name="time_signature"></param>
        /// <param name="creation_time"></param>

        public Song(string title, User creator,
                    string composer, string time_signature, DateTime creation_time)
        {
            this.Title = title;
            this.Composer = composer;
            this.TimeSignature = time_signature;
        }
    }
}
