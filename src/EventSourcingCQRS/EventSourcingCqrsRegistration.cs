using EventSourcingCQRS.Commanding;
using EventSourcingCQRS.Querying;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCQRS
{
    public static class EventSourcingCqrsRegistration
    {
        public static IServiceCollection AddEventSourcingCqrsHandlers(this IServiceCollection services)
        {
            services
                .Scan(selector =>
                {
                    selector.FromApplicationDependencies()
                        .AddClasses(filter =>
                        {
                            filter.AssignableTo(typeof(ICommandHandler<,>));
                        })
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime();
                });

            services
                .Scan(selector =>
                {
                    selector.FromAssemblies()
                        .AddClasses(filter =>
                        {
                            filter.AssignableTo(typeof(IQueryHandler<,>));
                        })
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime();
                });

            return services;
        }
    }
}