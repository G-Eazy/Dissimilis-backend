using System.Linq;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models;

namespace Dissimilis.DbContext
{
    public static class DissimilisSeeder
    {
        public static void SeedBasicData(DissimilisDbContext context)
        {
            if (context.Users.All(x => !x.IsSystemAdmin))
            {
                context.Users.Add(new User() { Name = "AdminUser", Email = "per.christian.kofstad@ciber.no", IsSystemAdmin=true });
            }

            context.SaveChanges();
        }
    }
}
