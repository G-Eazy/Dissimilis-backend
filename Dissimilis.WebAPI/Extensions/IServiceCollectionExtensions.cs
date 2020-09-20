using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Dissimilis.WebAPI.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices<T>(this IServiceCollection services)
            where T : class
        {
            services.Scan(scan =>
                scan
                    .FromAssembliesOf(typeof(T))

                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Factory")))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Policy")))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Context")))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("BackgroundTasks")))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                    .AddClasses().AsMatchingInterface()
            );
            return services;
        }

    }
}