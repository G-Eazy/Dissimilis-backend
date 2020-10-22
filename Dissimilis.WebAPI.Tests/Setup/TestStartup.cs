using System;
using Dissimilis.DbContext;
using Dissimilis.WebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            services.AddDbContext<DissimilisDbContext>(options =>
                    options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInMemoryDatabase(DbInitializer.MemoryDatabaseName));
        }

        public override void InitializeDb(IApplicationBuilder app, DissimilisDbContext context)
        {
            DbInitializer.Initialize(app.ApplicationServices);
        }

        public override void Migrate(DissimilisDbContext context)
        { }

        public override void AddAuthService(IServiceCollection services)
        {
            services.AddSingleton<TestServerFixture>();
            services.AddSingleton<IAuthService, TestAuthService>();
            services.AddSingleton<DbContext.DissimilisDbContextFactory>();
        }
    }
}
