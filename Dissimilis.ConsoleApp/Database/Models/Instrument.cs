using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class Instrument
    {
        [Key]
        public int InstrumentId { get; set; }

        public string InstrumentType { get; set; }

        public Instrument() { }

        public Instrument(string instrument)
        {
            this.InstrumentType = instrument;
        }

        public ICollection<Part> Parts { get; set; }
    }
}
