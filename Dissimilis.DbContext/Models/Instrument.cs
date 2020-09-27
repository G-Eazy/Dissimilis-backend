using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// Entity class for Instruemtn
    /// Contains Id, Name
    /// </summary>
    public class Instrument 
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// String containing the InstrumentType
        /// </summary>
        [MaxLength(250)]
        public string Name { get; set; }

        
        public ICollection<SongVoice> Parts { get; set; } = new List<SongVoice>();

        public Instrument() { }

        /// <summary>
        /// Instrument constructor
        /// </summary>
        /// <param name="instrument"></param>
        public Instrument(string instrument)
        {
            this.Name = instrument;
        }
    }
}
