namespace Dissimilis.DbContext.Models
{
    /// <summary>
    /// A many-to-many relationship table
    /// between usergroups and resources
    /// </summary>
    public class UserGroupResources
    {
        /// <summary>
        /// The UserGroupId 
        /// </summary>
        public int UserGroupId { get; set; }

        /// <summary>
        /// The usergroup associated with the userGroupId
        /// </summary>
        public UserGroup UserGroup { get; set; }

        /// <summary>
        /// The id of the Resource 
        /// </summary>
        public int ResourceId { get; set; }

        /// <summary>
        /// The resource associated with the ResourceId
        /// </summary>
        public Resource Resource { get; set; }

        /// <summary>
        /// empty constructor for group resources
        /// </summary>
        public UserGroupResources() { }

        /// <summary>
        /// Construcotr for usergroup taking in id of resource and group
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="userGroupId"></param>
        public UserGroupResources(int resourceId, int userGroupId)
        {
            this.ResourceId = resourceId;
            this.UserGroupId = userGroupId;
        }
    }
}
