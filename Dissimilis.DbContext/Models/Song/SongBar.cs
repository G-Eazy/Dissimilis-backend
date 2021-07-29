using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public int Position { get; set; }

        /// <summary>
        /// Is there a repetion sign before the bar 
        /// </summary>
        public bool RepBefore { get; set; }

        /// <summary>
        /// Is there a repetion sign after the bar
        /// </summary>
        public bool RepAfter { get; set; }

        /// <summary>
        /// If 0, there is no volta bracket. Otherwise it should follow an order
        /// A volta bracket is the equivalent to "hus" in Norwegian music theory.
        /// </summary>
        public int? VoltaBracket { get; set; }


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

        public SongBar(int position)
        {
            Position = position;
        }


        public SongBar Clone()
        {
            return new SongBar()
            {
                VoltaBracket = VoltaBracket,
                RepAfter = RepAfter,
                RepBefore = RepBefore,
                Position = Position,
                Notes = Notes.Select(n => n.Clone()).ToList()
            };
        }
    }
}
