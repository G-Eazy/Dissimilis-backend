using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dissimilis.ConsoleApp.Database
{
    class DissimilisDbContextOptions : DbContextOptionsBuilder
	{
		public DissimilisDbContextOptions() {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();
    
            
            var configuration = builder.Build();
            Console.WriteLine(configuration);
            var myConnString = configuration["default"];

            this.UseSqlServer(myConnString);
        }

	}

}

