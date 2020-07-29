using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// Entity class for Instruemtn
    /// Contains Id, Name
    /// </summary>
    public class Instrument : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// String containing the InstrumentType
        /// </summary>
        [MaxLength(250)]
        public string Name { get; set; }

        /// <summary>
        /// Empty constructor for Instrument
        /// </summary>
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
