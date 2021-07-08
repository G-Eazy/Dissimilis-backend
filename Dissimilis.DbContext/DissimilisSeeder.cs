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

            context.SaveChanges();


        }
    }
}
