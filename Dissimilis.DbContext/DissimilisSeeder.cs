using System.Linq;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models;

namespace Dissimilis.DbContext
{
    public static class DissimilisSeeder
    {
        public static void SeedBasicData(DissimilisDbContext context)
        {
            var user = context.Users.SingleOrDefault(x => x.Name == "AdminUser");
            if (user is null)
            {
                context.Users.Add(new User() { Name = "AdminUser", Email = "admin@support.no", isSystemAdmin=true });
            }
            var organisation = context.Organisations.SingleOrDefault(o => o.Name == "Norway");

            if(organisation is null)
            {
                context.Organisations.Add(new Organisation() { Name = "Norway" });
                var Organisation = context.Organisations.SingleOrDefault(x => x.Name == "Norway");
                var User = context.Users.SingleOrDefault(x => x.Name == "AdminUser");
                context.OrganisationUsers.Add(new OrganisationUser(Organisation, User, Role.Admin));
                var Admin = context.OrganisationUsers.SingleOrDefault(x => x.Organisation == organisation && x.User == User);
                organisation.Users.Add(Admin);
            
            }
            var organisation1 = context.Organisations
                .SingleOrDefault(o => o.Name == "Norway");
            var userOrganisations = context.Users
                .Where(u => u.Organisations.Count == 0)
                .Select(user => new OrganisationUser(organisation1, user, Role.Admin));
            organisation1.Users.AddRange(userOrganisations);

            context.SaveChanges();


        }
    }
}
