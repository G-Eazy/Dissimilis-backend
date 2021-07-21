using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Interfaces;
using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// The different sharing options a song could have:
    /// private: only you and people in sharedUsers
    /// Group: all in sharedUsers and sharedGroups
    /// Organisation: same as group + all in sharedOrganisation
    /// all: public song
    /// </summary>
    

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

        public DateTimeOffset? Deleted { get; set; }

        /// <summary>
        /// The composer of the song
        /// </summary>
        [MaxLength(100)]
        public string Composer { get; set; }

        /// <summary>
        /// The time signature of the song (NO: Taktart)
        ///
        /// From: https://snl.no/takt_-_musikk
        /// NO:  I begynnelsen av et musikkstykke angis taktarten. Nevneren i en brøk angir den noteverdien som brukes som taktdel eller telleenhet. Telleren angir antallet av slike taktdeler i en takt.
        ///
        /// Telleren
        /// </summary>
        [Range(2, 12)]
        public int Numerator { get; set; }
        /// <summary>
        /// See Numerator for more description
        ///
        /// Nevneren
        /// </summary>
        [Range(2, 8)]
        public int Denominator { get; set; }



        /// <summary>
        /// ID of the creator, eg. instructor
        /// </summary>
        public User Arranger { get; set; }
        public int? ArrangerId { get; set; }

        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }

        public User UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }

        /// <summary>
        /// Notes that describes the song
        /// </summary>
        public string SongNotes { get; set; }

        /// <summary>
        /// The speed of the song in BPM
        /// ( set a max range of 256) 
        /// </summary>
        [Range(0, 256)]
        public int? Speed { get; set; }

        /// <summary>
        /// how hard the song is to play
        /// (could change the range)
        /// </summary>
        [Range(1, 10)]
        public int? DegreeOfDifficulty { get; set; }

        public ICollection<SongVoice> Voices { get; set; } = new List<SongVoice>();

        /// <summary>
        /// The protection level of the song, a inner scope to easy set a song private
        /// </summary>
        public ProtectionLevels ProtectionLevel { get; set; }
        /// <summary>
        /// the users with write permission on the song
        /// </summary>
        public ICollection<SongSharedUser> SharedUsers { get; set; } = new List<SongSharedUser>();

        /// <summary>
        /// The groups with read permission on the song
        /// </summary>
        public ICollection<SongSharedGroup> SharedGroups { get; set; } = new List<SongSharedGroup>();

        /// <summary>
        /// The organisations with read permission on this song
        /// </summary>
        public ICollection<SongSharedOrganisation> SharedOrganisations { get; set; } = new List<SongSharedOrganisation>();
    }
}
