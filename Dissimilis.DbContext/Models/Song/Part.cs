using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Interfaces;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// This is a part, which is associated with a Song and contains bars
    /// </summary>
    public class Part : ICreatedAndUpdated
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The priority the voice has over other voices
        /// </summary>
        public int PartNumber { get; set; }


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
        public int InstrumentId { get; set; }

        /// <summary>
        /// The Song that this Part belongs too
        /// </summary>
        public Song Song { get; set; }
        public int SongId { get; set; }

        public ICollection<Bar> Bars { get; set; } = new List<Bar>();

        public Part() { }

        /// <summary>
        /// Constructor to create a part
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="instrumentId"></param>
        /// <param name="partNumber"></param>
        public Part(int songId, int instrumentId, byte partNumber)
        {
            SongId = songId;
            InstrumentId = instrumentId;
            PartNumber = partNumber;
        }


    }
}
