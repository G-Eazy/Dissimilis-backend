using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Interfaces;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// Norsk: Takt
    /// </summary>
    public class SongBar
    {
        /// <summary>
        /// The id of this bar
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// Priority of the bar in a spesific part
        /// </summary>
        public int BarNumber { get; set; }

        /// <summary>
        /// Is there a repetion sign before the bar 
        /// </summary>
        public bool RepBefore { get; set; }

        /// <summary>
        /// Is there a repetion sign after the bar
        /// </summary>
        public bool RepAfter { get; set; }

        /// <summary>
        /// if 0, there is no house. otherwise it should follow an order
        /// </summary>
        public int? House { get; set; }

    
        /// <summary>
        /// The part it is linked to
        /// and the corresponding PartId
        /// </summary>
        public SongVoice SongVoice { get; set; }
        public int SongVoiceId { get; set; }

        public ICollection<SongNote> Notes { get; set; } = new List<SongNote>();

        /// <summary>
        /// Empty constructor for Bar
        /// </summary>
        public SongBar() { }

       
      
    }
}
