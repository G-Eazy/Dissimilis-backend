using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Instrument
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// String containing the InstrumentType
        /// </summary>
        public string InstrumentType { get; set; }

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
            this.InstrumentType = instrument;
        }
    }
}
