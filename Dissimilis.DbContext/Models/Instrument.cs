using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.DbContext.Models
{
    public enum DefinedInstruments
    {
        [Description("Accordion")]
        Accordion = 0,

        [Description("Acoustic Guitar")]
        Acoustic_Guitar = 10,

        [Description("Bagpipes")]
        Bagpipes = 20,

        [Description("Banjo")]
        Banjo = 30,

        [Description("Bass Guitar")]
        Bass_Guitar = 40,

        [Description("Bongo Drums")]
        Bongo_Drums = 50,

        [Description("Bugle")]
        Bugle = 60,

        [Description("Cello")]
        Cello = 70,

        [Description("Clarinet")]
        Clarinet = 80,

        [Description("Cymbals")]
        Cymbals = 90,

        [Description("Drums")]
        Drums = 100,

        [Description("Electric Guitar")]
        Electric_Guitar = 110,

        [Description("Flute")]
        Flute = 120,

        [Description("French Horn")]
        French_Horn = 130,

        [Description("Harmonica")]
        Harmonica = 140,

        [Description("Keyboard")]
        Keyboard = 150,

        [Description("Maracas")]
        Maracas = 160,

        [Description("Organ")]
        Organ = 170,

        [Description("Pan Flute")]
        Pan_Flute = 180,

        [Description("Piano")]
        Piano = 190,

        [Description("Recorder")]
        Recorder = 200,

        [Description("Saxophone")]
        Saxophone = 210,

        [Description("Tambourine")]
        Tambourine = 220,

        [Description("Triangle")]
        Triangle = 230,

        [Description("Trombone")]
        Trombone = 240,

        [Description("Trumpet")]
        Trumpet = 250,

        [Description("Tuba")]
        Tuba = 260,

        [Description("Ukulele")]
        Ukulele = 270,

        [Description("Violin")]
        Violin = 280,

        [Description("Xylophone")]
        Xylophone = 290
    }

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
