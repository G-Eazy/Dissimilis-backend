using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Enums;
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
        [Required]
        public string Name { get; set; }

        public DefinedInstruments? DefinedInstrument { get; set; }
        /// <summary>
        /// the different songVoices in the whole database using this instrument
        /// </summary>
        public ICollection<SongVoice> SongVoices { get; set; } = new List<SongVoice>();

        public Instrument() { }
        public Instrument(DefinedInstruments instrument) {
            DefinedInstrument = instrument;
            Name = instrument.GetDescription();
        }

        /// <summary>
        /// Instrument constructor
        /// </summary>
        /// <param name="instrument"></param>
        public Instrument(string instrument)
        {
            Name = instrument;
        }
    }
}
