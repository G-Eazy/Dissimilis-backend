using Dissimilis.DbContext.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// the mapping between Organisation and user with the predefined enum role
    /// </summary>
    public class OrganisationUser
    {
        /// <summary>
        /// the organisation this user has the given role
        /// </summary>
        public Organisation Organisation { get; set; }
        public int OrganisationId { get; set; }

        /// <summary>
        /// the user with the given role in the organisation
        /// </summary>
        public User User { get; set; }
        public int UserId { get; set; }

        /// <summary>
        ///predefined roles; admin and instructor
        /// </summary>
        public Role Role { get; set; }

        public OrganisationUser() { }

    public OrganisationUser(int organisationId, int userId, Role role)
    {
        OrganisationId = organisationId;
        UserId = userId;
        Role = role;
    }
    }

}
