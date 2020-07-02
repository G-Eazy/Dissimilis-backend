using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dissimilis.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<DissimilisDbContext>(x => this.ConfigureDbOptions(ref x));
        
        }

        /// <summary>
        /// Configure the DbOptions. We get the connectionstring 
        /// that is placed in appsettings.Development.json
        /// </summary>
        /// <param name="dbCob"></param>
        protected virtual void ConfigureDbOptions(ref DbContextOptionsBuilder dbCob)
        {
            var conn = this.Configuration.GetConnectionString("default");
            if (conn is null)
            {
                throw new Exception("Please set the connection string called default");
            }

            dbCob.UseSqlServer(conn);
        }

        protected virtual void AddContextData(DissimilisDbContext dbContext)
        {
            //dont run if there are any users in the database
            //No points as it will already have run the seeder.
            if(!dbContext.Users.Any()){
                DissimilisSeeder.SeedData(dbContext);
            }
            //To do seeding of data
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //create a service scope
            using(var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                using var context = serviceScope.ServiceProvider.GetRequiredService<DissimilisDbContext>();
                context.Database.EnsureCreated();
                this.AddContextData(context);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
