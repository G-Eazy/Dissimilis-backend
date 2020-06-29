using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Part
    {
        [Key]
        public int PartId { get; set; }

        /// <summary>
        /// The Song that this Part belongs too
        /// </summary>
        public Song Song { get; }
        public int SongId { get; }

        /// <summary>
        /// ID of the creator, eg. instructor
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// A string containing name of instrument(should be it's own entity)
        /// </summary>
        [Column(TypeName = "text")]
        public string Instrument { get; set; }

        /// <summary>
        /// Time and date of creation of voice
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// The priority the voice has over other voices
        /// </summary>
        public int Priority { get; set; }

    }
}
