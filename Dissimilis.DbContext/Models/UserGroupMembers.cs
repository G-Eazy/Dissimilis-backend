namespace Dissimilis.DbContext.Models
{
    public class UserGroupMembers
    {
        //Setting up a many-to-many relationshop between 
        //Usergroup and User.
        /// <summary>
        /// the user id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The user that is associated with the id
        /// </summary>
        public User User{ get; set; }

        /// <summary>
        /// The user group id
        /// </summary>
        public int UserGroupId { get; set; }

        /// <summary>
        /// the user group associated with the id
        /// </summary>
        public UserGroup UserGroup { get; set; }

        /// <summary>
        /// Empty constructor for usergroup members
        /// </summary>
        public UserGroupMembers() { }

        /// <summary>
        /// Constructor for usergroup members with params
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGroupId"></param>
        public UserGroupMembers(int userId, int userGroupId)
        {
            this.UserId = userId;
            this.UserGroupId = userGroupId;
        }
    }
}
