using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dissimilis.DbContext.Interfaces;

namespace Dissimilis.DbContext.Models.Song
{
    /// <summary>
    /// This is the bar, which is associated with a Part (norsk: Stemme)
    /// </summary>
    public class Bar
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
        public Part Part { get; set; }
        public int PartId { get; set; }

        public ICollection<Note> Notes { get; set; } = new List<Note>();

        /// <summary>
        /// Empty constructor for Bar
        /// </summary>
        public Bar() { }

        public Bar(INewBar bar)
        {
            this.BarNumber = bar.BarNumber;
            this.PartId = bar.PartId;
            this.RepAfter = bar.RepAfter;
            this.RepBefore = bar.RepBefore;
            this.House = bar.House;
        }

      
    }
}
