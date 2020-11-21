using System;
using System.IO;
using System.Reflection;
using Dissimilis.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Dissimilis.WebAPI
{
    public static class SwaggerConfiguration
    {
        public static void SetSwaggerGenOptions(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dissimilis API",
                Version = "v1",
                Description = "Purely for development and testing purposes.",
                Contact = new OpenApiContact()
                {
                    Name = "Experis Ciber",
                    Email = "kontakt@ciber.no"
                }
            });

            options.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}");

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.CustomSchemaIds(x => x.FullName);

            options.OperationFilter<Authentication.AddRequiredHeaderParameter>();
        }

        public static void SetSwaggerUiOptions(SwaggerUIOptions c)
        {
            var enviroment = "";
            try
            {
                enviroment = " - " + ConfigurationInfo.GetEnviromentVariable();
            }
            catch { }

            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Dissimilis - v1");
            c.DisplayRequestDuration();

            c.DefaultModelsExpandDepth(0);
        }
    }
}
