using System;
using System.Collections.Generic;
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
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            //Set the comments path for the Swagger JSON and UI.
            options.IncludeXmlComments(xmlPath);

            options.CustomSchemaIds(x => x.FullName);


            // Adding security token
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,

            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            {  new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            }, new List<string>() } });
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
