using Dissimilis.DbContext.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.DbContext.Models
{ 
    /// <summary>
    /// the mapping between Group and user with the predefined enum role
    /// </summary>
    public class GroupUser
    {
        /// <summary>
        /// the Group this user has the given role
        /// </summary>
        public Group Group { get; set; }
        public int GroupId { get; set; }

        /// <summary>
        /// the user with the given role in the Group
        /// </summary>
        public User User { get; set; }
        public int UserId { get; set; }

        /// <summary>
        ///predefined roles; admin and instructor
        /// </summary>
        public Role Role { get; set; }

        public GroupUser() { }

        public GroupUser(int groupId, int userId, Role role)
        {
            GroupId = groupId;
            UserId = userId;
            Role = role;
        }
    }
}
