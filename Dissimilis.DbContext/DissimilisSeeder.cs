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
                var adminUser = context.Users.SingleOrDefault(u => u.Email == "per.christian.kofstad@live.no");
                if (adminUser == null)
                {
                    context.Users.Add(new User()
                    {
                        MsId = "d47c77e14d05511c",
                        Name = "AdminUser",
                        Email = "per.christian.kofstad@live.no",
                        IsSystemAdmin = true
                    });
                }
                else
                {
                    adminUser.IsSystemAdmin = true;
                }
            }
            context.SaveChanges();
        }
    }
}
