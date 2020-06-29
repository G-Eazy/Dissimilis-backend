using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Voice
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// ID of the creator, eg. instructor
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// ID of the music sheet associated with this voice
        /// </summary>
        public int MusicSheetId { get; set; }

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
