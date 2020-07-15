using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Models
{
    /// <summary>
    /// This entity class will be used when there are updates 
    /// or additions to other entities, by updated the time created/updated
    /// and who created/updated
    /// </summary>
    public abstract class BaseEntity
    {

        /// <summary>
        /// Who created this attribute
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Who updated this attribute last
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// When was this attribute created
        /// </summary>  
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// What time was this attribute updated
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
