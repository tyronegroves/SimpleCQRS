using Azure.Data.Tables;
using EventSourcingCQRS.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCQRS.EventStore.AzureTableStorage
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAzureTableEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();

            services
                .AddTransient<TableClient>(tc =>
                {
                    var connectionString =
                        "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://localhost:10002";
                    var tableClient = new TableClient(connectionString, "Events");
                    tableClient.CreateIfNotExists();
                    return tableClient;
                });
            services
                .AddTransient<IEventStore, AzureTableEventStore>();

            services
                .AddTransient<ISnapshotStore, AzureTableSnapshotStore>();
        }
    }
}