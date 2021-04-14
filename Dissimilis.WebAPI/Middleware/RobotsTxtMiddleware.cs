using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Middleware
{
    public static class RobotsTxtMiddlewareExtensions
    {
        public static IApplicationBuilder UseRobotsTxt(
            this IApplicationBuilder builder,
            IWebHostEnvironment env,
            string rootPath = null
            )
        {
            return builder.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/robots.txt"), b =>
                b.UseMiddleware<RobotsTxtMiddleware>(env.EnvironmentName, rootPath ?? env.ContentRootPath));
        }
    }
    public class RobotsTxtMiddleware
    {
        const string Default =
            @"User-Agent: *\nDisallow: /";

        private readonly RequestDelegate next;
        private readonly string environmentName;
        private readonly string rootPath;

        public RobotsTxtMiddleware(
            RequestDelegate next,
            string environmentName,
            string rootPath
            )
        {
            this.next = next;
            this.environmentName = environmentName;
            this.rootPath = rootPath;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/robots.txt"))
            {
                var generalRobotsTxt = Path.Combine(rootPath, "robots.txt");
                string output;
                if(File.Exists(generalRobotsTxt))
                {
                    output = await File.ReadAllTextAsync(generalRobotsTxt);
                }
                else
                {
                    output = Default;
                }

                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(output);
            }
            else
            {
                await next(context);
            }
        }
    }
}
