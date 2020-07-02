using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Models
{
    public class UserGroupMembers
    {
        //Setting up a many-to-many relationshop between 
        //Usergroup and User.
        public int UserId { get; set; }
        public User User{ get; set; }
        public int UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }

        public UserGroupMembers() { }

        public UserGroupMembers(int userId, int userGroupId)
        {
            this.UserId = userId;
            this.UserGroupId = userGroupId;
        }
    }
}
