using EventSourcingCQRS.Eventing;
using EventSourcingCQRS.EventStore.SqlServer.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCQRS.EventStore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSqlServerEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();
            services
                .AddTransient<ISnapshotStore, SqlServerSnapshotStore>();

            services
            .AddSingleton<SqlServerConfiguration>(s =>
              new SqlServerConfiguration(
                  configuration.GetConnectionString("EventStore")));

            services
                .AddTransient<ISnapshotStore, SqlServerSnapshotStore>();

            services
                .AddTransient<IEventStore, SqlServerEventStore>();
        }
    }
}