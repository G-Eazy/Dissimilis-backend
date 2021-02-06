using System;
using System.Linq;
using Dissimilis.DbContext;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    public class DbInitializer
    {
        private static DissimilisDbContext _dbContext;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _dbContext = (DissimilisDbContext)serviceProvider.GetService(typeof(DissimilisDbContext));
            _dbContext.Database.EnsureCreated();

            SeedTestUser();

            _dbContext.SaveChanges();
        }

        private static void SeedTestUser()
        {
            var user = _dbContext.Users.FirstOrDefault(uid =>
                uid.Email == TestServerFixture.GetDefaultTestUser().Email);
            if (user == null)
            {
                user = TestServerFixture.GetDefaultTestUser();
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                TestServerFixture.CurrentUserId = user.Id;
            }
        }


    }
}
