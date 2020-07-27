using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// This is a part, which is associated with a Song and contains bars
    /// </summary>
    public class Part : BaseEntity, IPart
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Song that this Part belongs too
        /// </summary>
        public Song Song { get; }

        /// <summary>
        /// The foregin key of the Song it belongs too
        /// </summary>
        public int SongId { get; set; }

        /// <summary>
        /// The instrument entity of this Part
        /// </summary>
        public Instrument Instrument { get; set; }

        /// <summary>
        /// The id of the corresponding instrument
        /// </summary>
        public int InstrumentId { get; set; }

        /// <summary>
        /// The priority the voice has over other voices
        /// </summary>
        public ushort PartNumber { get; set; }

        /// <summary>
        /// Empty constructor for Part.cs
        /// </summary>
        public Part() { }

        /// <summary>
        /// Constructor to create a part
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="instrumentId"></param>
        /// <param name="partNumber"></param>
        public Part(int songId, int instrumentId, ushort partNumber)
        {
            this.SongId = songId;
            this.InstrumentId = instrumentId;
            this.PartNumber = partNumber;
        }
    }
}
