using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.WebAPI.Database.Models
{
    public class Part
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The Song that this Part belongs too
        /// </summary>
        public Song Song { get; }

        /// <summary>
        /// The foregin key of the Song it belongs too
        /// </summary>
        public int SongId { get; set; }

        /// <summary>
        /// A string containing name of instrument(should be it's own entity)
        /// </summary>
        public Instrument Instrument { get; set; }

        /// <summary>
        /// The id of the corresponding instrument
        /// </summary>
        public int InstrumentId { get; set; }

        /// <summary>
        /// Time and date of creation of voice
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// The priority the voice has over other voices
        /// </summary>
        [Column(TypeName = "TINYINT")]
        public int Priority { get; set; }

    }
}
