using EventSourcingCQRS.Eventing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCQRS.EventStore.CosmosDb
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCosmosDbEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();

            var cosmosClientOptions = new CosmosClientOptions()
            {
                //Allow insecure (non https) access to the Emulator
                HttpClientFactory = () =>
                {
                    HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    return new HttpClient(httpMessageHandler);
                },
                ConnectionMode = ConnectionMode.Gateway,
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            services
                .AddSingleton(s =>
                    new CosmosDbConfiguration(
                        configuration.GetConnectionString("EventStore")));

            services
                .AddSingleton<IEventStore, CosmosDbEventStore>(serviceProvider =>
                {
                    var cosmosDbConfig = new CosmosDbConfiguration(configuration.GetConnectionString("EventStore"));
                    var cosmosDbConnectionStringBuilder = CosmosDbConnectionStringBuilder.ParseConnectionString(cosmosDbConfig.ConnectionString);
                    var cosmosClient = new CosmosClient(cosmosDbConfig.ConnectionString, cosmosClientOptions);
                    var databaseName = cosmosDbConnectionStringBuilder.DatabaseName;

                    //The Cosmos library only has async methods which we need to run from here, a sync method
                    // The solution chosen is documented on stack overflow
                    //https://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c
                    var databaseResponse = Task.Run(() => cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName)).GetAwaiter().GetResult();

                    var database = cosmosClient.GetDatabase(databaseName);

                    //////var uniqueKey = new UniqueKey();
                    //////uniqueKey.Paths.Add("/eventData/Sequence");
                    var containerProperties = new ContainerProperties()
                    {
                        Id = "Events", //Equates to table name
                        //////UniqueKeyPolicy = new UniqueKeyPolicy()
                        //////{
                        //////    UniqueKeys = { uniqueKey }
                        //////},
                        PartitionKeyPath = "/eventData/AggregateRootId" 
                    };

                    var containerResponse = Task.Run(() => database.CreateContainerIfNotExistsAsync(containerProperties)).GetAwaiter().GetResult();

                    var container = database.GetContainer(containerProperties.Id);
                    var serialiser = new JsonDomainEventSerializer();
                    return new CosmosDbEventStore(cosmosClient, serialiser, database, container);
                });

            services
                .AddTransient<ISnapshotStore, CosmosDbSnapshotStore>();
        }
    }
}