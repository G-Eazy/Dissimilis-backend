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

            SeedSupplementTestUsers();

            _dbContext.SaveChanges();

            SeedTestOrganisation();

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

        private static void SeedSupplementTestUsers()
        {
            foreach (var newSuppUser in TestServerFixture.GetSupplementedTestUsers())
            {
                var user = _dbContext.Users.FirstOrDefault(dbUser =>
                    dbUser.Email == newSuppUser.Email);
                if (user == null)
                {
                    user = newSuppUser;
                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                }
            }
        }

        private static void SeedTestOrganisation()
        {
            var organisation = _dbContext.Organisations.FirstOrDefault(org =>
                org.Name == TestServerFixture.GetDefaultTestOrganisation().Name);
            if (organisation == null)
            {
                //Create new organisation and add to db.
                organisation = TestServerFixture.GetDefaultTestOrganisation();

                _dbContext.Organisations.Add(organisation);
                _dbContext.SaveChanges();

                //Fetch the currentuser to use as admin user in organisation.
                var adminUser = _dbContext.Users.SingleOrDefault(user =>
                    user.Id == TestServerFixture.CurrentUserId);

                //Create the organisation user with admin role, and add to db.
                var orgUser = TestServerFixture.GetDefaultTestOrganisationUser();
                orgUser.User = adminUser;
                orgUser.Organisation = organisation;
                orgUser.UserId = adminUser.Id;
                orgUser.OrganisationId = organisation.Id;

                adminUser.Organisations.Add(orgUser);
                organisation.Users.Add(orgUser);

                _dbContext.OrganisationUsers.Add(orgUser);
                _dbContext.SaveChanges();

            }
        }

    }
}
