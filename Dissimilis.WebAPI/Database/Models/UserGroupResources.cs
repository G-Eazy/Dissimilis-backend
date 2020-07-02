using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Models
{
    public class UserGroupResources
    {
        public int UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}
