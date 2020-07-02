using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.Database.Models
{
    public class Instrument : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// String containing the InstrumentType
        /// </summary>
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
