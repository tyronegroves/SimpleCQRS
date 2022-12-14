using EventSourcingCQRS.Commanding;
using EventSourcingCQRS.Eventing;
using EventSourcingCQRS.Querying;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventSourcingCQRS
{
    public static class EventSourcingCqrsRegistration
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            services
                .TryAddSingleton<ICommandDispatcher, CommandDispatcher>();

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

            return services;
        }

        public static IServiceCollection AddEventHandlers(this IServiceCollection services)
        {
            services
                .Scan(selector =>
                {
                    selector.FromApplicationDependencies()
                        .AddClasses(filter =>
                        {
                            filter.AssignableTo(typeof(IHandleDomainEvents<>));
                        })
                        .AsImplementedInterfaces()
                        //.WithSingletonLifetime();
                        .WithScopedLifetime();
                });

            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {
            services
                .TryAddSingleton<IQueryDispatcher, QueryDispatcher>();

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