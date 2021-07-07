using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// a table for the system admin for the whole system
    /// </summary>
    public class SystemAdmin
    {
        /// <summary>
        /// the id of the row
        /// </summary>
        [Key]
        public int Id;

        /// <summary>
        /// The user with system admin rights
        /// </summary>
        public User User;
        public int? UserId;
    }
}
