using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// This is the bar, which is associated with a Part (norsk: Stemme)
    /// </summary>
    public class Bar : BaseEntity, IBar
    {
        /// <summary>
        /// The id of this bar
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The part it is linked to
        /// and the corresponding PartId
        /// </summary>
        public Part Part { get; set; }

        /// <summary>
        /// THe ID for the corresponding Part 
        /// </summary>
        public int PartId { get; set; }

        /// <summary>
        /// Priority of the bar in a spesific part
        /// </summary>
        public byte BarNumber { get; set; }

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
        public byte House { get; set; }

        /// <summary>
        /// Empty constructor for Bar
        /// </summary>
        public Bar() { }

        public Bar(byte barNumber, int partId)
        {
            this.BarNumber = barNumber;
            this.PartId = partId;
        }
    }
}
