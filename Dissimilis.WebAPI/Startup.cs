using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Experis.Ciber.Web.API.Middleware;
using Dissimilis.WebAPI.Authentication;

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "DissAPI",
                    Version = "v1"
                });

                //This can be commented out if you want to
                c.OperationFilter<AddRequiredHeaderParameter>();

                var XMLFile = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                var XMLPath = Path.Combine(AppContext.BaseDirectory, XMLFile);
                c.IncludeXmlComments(XMLPath);
            });

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
                throw new Exception("The provided connection string is not valid");
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
            app.UseCors(c =>
            {
                c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
            // Only for week 31
            app.UseDeveloperExceptionPage();
            
            if (!env.IsDevelopment())
                app.UseHttpsRedirection();
            
            app.UseSwagger();

            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DissAPI V1");
                
            });

            app.UseWebUserAuthentication();
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
