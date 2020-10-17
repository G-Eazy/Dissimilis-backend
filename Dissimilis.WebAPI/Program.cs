using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Dissimilis.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration() // Necessary for Azure.
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var configSectionForLogging = hostingContext.Configuration.GetSection("Logging");
                    logging.AddFilter("", Enum.Parse<LogLevel>(configSectionForLogging["LogLevel:Default"] ?? "Information"));
                    logging.AddFilter("Microsoft", Enum.Parse<LogLevel>(configSectionForLogging["LogLevel:Microsoft"] ?? "Warning"));
                    logging.AddFilter("System", Enum.Parse<LogLevel>(configSectionForLogging["LogLevel:System"] ?? "Warning"));
                })
                .UseStartup<Startup>();
    }
}
