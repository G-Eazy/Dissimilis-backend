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
                var adminUser = context.Users.SingleOrDefault(u => u.Email == "per.christian.kofstad@ciber.no");

                if(adminUser == null)
                {
                    context.Users.Add(new User() { Name = "AdminUser", Email = "per.christian.kofstad@ciber.no", IsSystemAdmin=true });
                }
                else
                {
                    adminUser.IsSystemAdmin = true;
                }
            }
            var summerIntern = context.Users.SingleOrDefault(u => u.Email == "carl.valdemar.ebbesen@ciber.no");
            if(summerIntern != null)
            {
                summerIntern.IsSystemAdmin = true;
            }
            var oleSysAdmin = context.Users.SingleOrDefault(u => u.Email == "ole.jonas.liahagen@ciber.no");
            if (oleSysAdmin != null)
            {
                oleSysAdmin.IsSystemAdmin = true;
            }
            context.SaveChanges();
        }
    }
}
