using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Interfaces;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// Norsk: Partitur / Stemme
    /// </summary>
    public class SongVoice : ICreatedAndUpdated
    {
        [Key]
        public int Id { get; set; }
        public string VoiceName { get; set; }
        public int VoiceNumber { get; set; }
        public bool IsMainVoice { get; set; }

        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public User UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }

        /// <summary>
        /// The instrument entity of this Part
        /// </summary>
        public Instrument Instrument { get; set; }
        public int? InstrumentId { get; set; }

        /// <summary>
        /// The Song that this Part belongs too
        /// </summary>
        public Song Song { get; set; }
        public int SongId { get; set; }

        public ICollection<SongBar> SongBars { get; set; } = new List<SongBar>();

        public SongVoice() { }

        /// <summary>
        /// Constructor to create a part
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="instrumentId"></param>
        /// <param name="voiceNumber"></param>
        public SongVoice(int songId, int instrumentId, int voiceNumber, string voiceName)
        {
            SongId = songId;
            InstrumentId = instrumentId;
            VoiceNumber = voiceNumber;
            VoiceName = voiceName;
        }

    }
}
