using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// the mapping between Organisation and user with the predefined enum role
    /// </summary>
    public class OrganisationUser
    {
        /// <summary>
        /// the key
        /// </summary>
        [Key]
        public int Id;

        /// <summary>
        /// the organisation this user has the given role
        /// </summary>
        public Organisation Organisation;
        public int? OrganisationId;

        /// <summary>
        /// the user with the given role in the organisation
        /// </summary>
        public User User;
        public int? UserId;

        /// <summary>
        ///predefined roles; admin and instructor
        /// </summary>
        public Role Role;
    }
}
