using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Models
{
    public abstract class BaseEntity
    {
/*        /// <summary>
        /// Who created this attribute
        /// </summary>
        public int CreatedBy { get; set; }
        public User CreatedById { get; set; }

        /// <summary>
        /// Who updated this attribute last
        /// </summary>
        public int UpdatedBy { get; set; }
        public User UpdatedById { get; set; }*/

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
