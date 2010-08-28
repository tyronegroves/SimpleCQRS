using System;
using System.Linq;
using System.Reflection;
using MongoDB;
using MongoDB.Configuration;
using MongoDB.Configuration.Builders;
using SimpleCqrs.Domain;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore.MongoDb
{
    public class MongoSnapshotStore : ISnapshotStore
    {
        private static readonly MethodInfo MapMethod = typeof(MappingStoreBuilder).GetMethod("Map", Type.EmptyTypes);
        private readonly IMongoDatabase database;

        public MongoSnapshotStore(string connectionString, ITypeCatalog snapshotTypeCatalog)
        {
            var configuration = BuildMongoConfiguration(snapshotTypeCatalog, connectionString);
            var mongo = new Mongo(configuration);
            mongo.Connect();

            database = mongo.GetDatabase("snapshotstore");
        }

        private static MongoConfiguration BuildMongoConfiguration(ITypeCatalog snapshotTypeCatalog, string connectionString)
        {
            var configurationBuilder = new MongoConfigurationBuilder();
            configurationBuilder.ConnectionString(connectionString);
            configurationBuilder.Mapping(mapping =>
                                             {
                                                 mapping.DefaultProfile(profile => profile.SubClassesAre(type => type.IsSubclassOf(typeof(Snapshot))));
                                                 snapshotTypeCatalog
                                                     .GetDerivedTypes(typeof(Snapshot))
                                                     .ForEach(type => MapEventType(type, mapping));
                                             });

            return configurationBuilder.BuildConfiguration();
        }

        public Snapshot GetSnapshot(Guid aggregateRootId)
        {
            var snapshotsCollection = database.GetCollection<Snapshot>("snapshots").Linq();
            return (from snapshot in snapshotsCollection
                    where snapshot.AggregateRootId == aggregateRootId
                    select snapshot).SingleOrDefault();
        }

        public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot
        {
            var snapshotsCollection = database.GetCollection<TSnapshot>("snapshots");
            snapshotsCollection.Save(snapshot);
        }
        
        private static void MapEventType(Type type, MappingStoreBuilder mapping)
        {
            MapMethod.MakeGenericMethod(type)
                .Invoke(mapping, new object[] { });
        }
    }
}