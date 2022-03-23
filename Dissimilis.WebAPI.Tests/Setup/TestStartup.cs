using System.Data.Common;
using Dissimilis.DbContext;
using Dissimilis.WebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    public class TestStartup : Startup
    {

        public TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env, null)
        {
        }


        public override void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<DissimilisDbContext>(options => options
                    .UseSqlite(CreateInMemoryDatabase()));        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = ":memory:",
                ForeignKeys = true,
                Cache = SqliteCacheMode.Private,
                Mode = SqliteOpenMode.Memory
            }.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        }


        public override void InitializeDb(IApplicationBuilder app, DissimilisDbContext context)
        {
            DbInitializer.Initialize(app.ApplicationServices);
            context.Database.EnsureCreated();
        }

        public override void Migrate(DissimilisDbContext context)
        {
        }

        public override void AddAuthService(IServiceCollection services)
        {
            services.AddSingleton<TestServerFixture>();
            services.AddSingleton<IAuthService, TestAuthService>();
           // services.AddSingleton<DissimilisDbContextFactory>();
        }
    }
}
