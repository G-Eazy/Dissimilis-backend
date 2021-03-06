using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Dissimilis.Configuration;
using Dissimilis.DbContext;
using Dissimilis.WebAPI.CustomFilters;
using Dissimilis.WebAPI.Extensions;
using Dissimilis.WebAPI.Middleware;
using Dissimilis.WebAPI.Services;
using Experis.Ciber.Web.API.Middleware;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using Newtonsoft.Json.Serialization;

namespace Dissimilis.WebAPI
{
    public class Startup
    {
        private const string CORSPOLICY = "CorsPolicy";
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<Startup> _logger;

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;
            StaticConfig = configuration;
            _environment = env;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLogging(config =>
            {
                var logconfig = Configuration.GetSection("Logging");
                config.ClearProviders();
                config.AddConfiguration(logconfig);
                config.AddConsole();

                config.AddFilter<FileLoggerProvider>("", Enum.Parse<LogLevel>(logconfig["LogLevel:Default"] ?? "Information"));
                config.AddFilter<FileLoggerProvider>("Microsoft", Enum.Parse<LogLevel>(logconfig["LogLevel:Microsoft"] ?? "Warning"));
                config.AddFilter<FileLoggerProvider>("System", Enum.Parse<LogLevel>(logconfig["LogLevel:System"] ?? "Warning"));

                config.AddAzureWebAppDiagnostics();
            });
            services.AddSingleton(new ConfigurationInfo(Configuration, _logger));

            ConfigurationInfo.IsConfigurationHealthOk();

            services.AddSingleton<ITelemetryInitializer>(new LoggingInitializer(Configuration["Logging:ApplicationInsights:RoleName"]));
            services.AddApplicationInsightsTelemetry();
            TelemetryDebugWriter.IsTracingDisabled = true;
            
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            ConfigureDatabase(services);
            services.AddTransient<DissimilisDbContextFactory>();
            
            services.AddServices<Startup>();
            AddAuthService(services);
            services.AddControllers();
            services.AddSwaggerGen(SwaggerConfiguration.SetSwaggerGenOptions);
            services.AddCors(options =>
            {
                options.AddPolicy(CORSPOLICY,
                    builder => builder.WithOrigins(
                           "https://localhost:3000",
                           "http://localhost:5000",
                           "https://localhost:5001",
                           "https://dissimilisfargenotasjon.azurewebsites.net",
                           "https://dissimilis-pwa-dev.azurewebsites.net",
                           "https://dissimilis-pwa-test.azurewebsites.net",
                           "https://dissimilis-pwa-prod.azurewebsites.net",
                           ConfigurationInfo.GetFrontendBaseUrl()
                           )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            services.AddMvcCore(options =>
                {
                    options.Filters.Add(typeof(ValidateModelStateAttribute));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddApplicationPart(typeof(IServiceCollectionExtensions).Assembly)
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver());
        }


        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = ConfigurationInfo.GetSqlConnectionString();
            services.AddDbContext<DissimilisDbContext>(options => options.UseSqlServer(connectionString));
        }

        private static void EnsureNorwegianCulture(IApplicationBuilder app)
        {
            var requestOpt = new RequestLocalizationOptions();
            requestOpt.SupportedCultures = new List<CultureInfo>
            {
                new CultureInfo(ConfigurationInfo.NORWEGIAN_CUTURE)
            };
            requestOpt.SupportedUICultures = new List<CultureInfo>
            {
                new CultureInfo(ConfigurationInfo.NORWEGIAN_CUTURE)
            };
            requestOpt.RequestCultureProviders.Clear();
            requestOpt.RequestCultureProviders.Add(new SingleCultureProvider());
            app.UseRequestLocalization(requestOpt);
        }

        private class SingleCultureProvider : IRequestCultureProvider
        {
            public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
            {
                return Task.Run(() => new ProviderCultureResult(ConfigurationInfo.NORWEGIAN_CUTURE, ConfigurationInfo.NORWEGIAN_CUTURE));
            }
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DissimilisDbContext dbContext)
        {
            EnsureNorwegianCulture(app);

            Migrate(dbContext);
            InitializeDb(app, dbContext);
            
            app.UseRouting();
            app.UseCors(CORSPOLICY);

            app.UseSwagger();
            app.UseSwaggerUI(SwaggerConfiguration.SetSwaggerUiOptions);

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (ConfigurationInfo.IsLocalDebugBuild())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!ConfigurationInfo.IsAutomatedTestingMode())
            {
                app.UseWebUserAuthentication();
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        public virtual void AddAuthService(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }

        public virtual void Migrate(DissimilisDbContext context)
        {

            context.Database.Migrate();
        }

        public virtual void InitializeDb(IApplicationBuilder app, DissimilisDbContext context)
        {

            DissimilisSeeder.SeedBasicData(context);
            DBMigrationJobs.MigrateFromOldToNewChordFormatAsync(context);
            DBMigrationJobs.SetInstrumentAsVoiceName(context);
            DBMigrationJobs.DeleteOldInstruments(context);
            DBMigrationJobs.CreateNewInstruments(context);
            DBMigrationJobs.UpdateExistingInstrumentName(context);
            DBMigrationJobs.SetAllSongsToPublic(context);
            if (ConfigurationInfo.IsLocalDebugBuild())
            {
                // local seeding
            }
        }
    }
}
