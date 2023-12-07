using EventSourcingCQRS.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EventSourcingCQRS.EventStore.MongoDb
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoDbEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDB STUFF
            //https://stackoverflow.com/questions/63443445/trouble-with-mongodb-c-sharp-driver-when-performing-queries-using-guidrepresenta
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
            BsonSerializer
                .RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            services
                .AddSingleton<IMongoClient, MongoClient>(s =>
                    new MongoClient(
                        configuration.GetConnectionString(
                            "EventStore")));
            services
                .AddSingleton<IMongoEventStoreConfiguration, MongoEventStoreConfiguration>();

            services
                .AddTransient<IEventStore, MongoEventStore>();
            services
                .AddTransient<ISnapshotStore, MongoSnapshotStore>()
                .AddSingleton<IMongoSnapshotStoreConfiguration, MongoSnapshotStoreConfiguration>();
            //services
            //    .AddPollyPolicies();
        }
    }
}