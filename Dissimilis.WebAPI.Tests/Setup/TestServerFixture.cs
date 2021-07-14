using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Dissimilis.DbContext.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }
        public static int CurrentUserId = GetDefaultTestUser().Id;

        public TestServerFixture()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "TestEnviromentVariables.json"), optional: true)
                .Build();

            _testServer = new TestServer(
                new WebHostBuilder()
                    .UseConfiguration(config)
                    .UseStartup<TestStartup>());

            Client = _testServer.CreateClient();
        }

        internal IServiceProvider GetServiceProvider()
        {
            return _testServer.Host.Services;
        }

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }

        public static User GetDefaultTestUser()
        {
            return new User()
            {
                Name = "Testuser",
                Email = "test@test.no",
                IsSystemAdmin = true,
            };
        }

        public static List<User> GetSupplementedTestUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    Name = "SupUser1",
                    Email = "supUser1@test.no",
                },
                new User()
                {
                    Name = "SupUser2",
                    Email = "supUser2@test.no",
                },
                new User()
                {
                    Name = "SupUser3",
                    Email = "supUser3@test.no",
                }
            };
        }

        public void ChangeCurrentUserId(int newCurrentUserId)
        {
            TestServerFixture.CurrentUserId = newCurrentUserId;
        }

        public static Organisation GetDefaultTestOrganisation()
        {
            return new Organisation()
            {
                Name = "Norway",
            };
        }

        public static List<Organisation> GetSupplementOrganisations()
        {
            return new List<Organisation>(){
                new Organisation()
                {
                    Name = "Spain"
                },
                new Organisation()
                {
                    Name = "Guatamala"
                },
                new Organisation()
                {
                    Name = "France"
                }
            };
        }

        public static OrganisationUser GetDefaultTestOrganisationUser()
        {
            return new OrganisationUser()
            {
                Role = DbContext.Models.Enums.Role.Admin,
            };
        }

        public static List<OrganisationUser> GetSupplementedTestOrganisationUsers()
        {
            return new List<OrganisationUser>()
            {
                new OrganisationUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new OrganisationUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new OrganisationUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
            };
        }

        public static List<Group> GetTestGroups()
        {
            return new List<Group>()
            {
                new Group()
                {
                    Name = "Dissimilis Sandvika",
                },
                new Group()
                {
                    Name = "Dissimilis Bergen",
                },new Group()
                {
                    Name = "Dissimilis Trondheim",
                },
            };
        }

        public static GroupUser GetDefaultTestGroupUser()
        {
            return new GroupUser()
            {
                Role = DbContext.Models.Enums.Role.Admin,
            };
        }

        public static List<GroupUser> GetSupplementedTestGroupUsers()
        {
            return new List<GroupUser>()
            {
                new GroupUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new GroupUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new GroupUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
            };
        }
    }
}
