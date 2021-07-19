using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class UserExtension
    {

        public static bool IsSystemAdmin(this User user)
        {
            return user.IsSystemAdmin;
        }
        public static bool IsAdminForGroup(this User user, int groupId)
        {
            return user.Groups.Any(x => x.GroupId == groupId && x.Role == Role.Admin);
        }

        public static bool IsAdminForOrganisation(this User user, int organisationId)
        {
            return user.Organisations.Any(x => x.OrganisationId == organisationId && x.Role == Role.Admin);
        }
        public static List<int> GetAllOrganisationIds(this User user)
        {
            return user.Organisations.Select(o => o.OrganisationId).ToList();
        }

        public static List<int> GetAllGroupIds(this User user)
        {
            return user.Groups.Select(g => g.GroupId).ToList();
        }
    }
}
