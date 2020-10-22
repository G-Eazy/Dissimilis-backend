using System;
using System.IO;
using System.Net.Http;
using Dissimilis.DbContext;
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
                Email = "test@test.no"
            };
        }


    }
}
