using System;
using System.Linq;
using MongoDB;
using MongoDB.Configuration;
using SimpleCqrs.Domain;

namespace SimpleCqrs.EventStore.MongoDb
{
    public class MongoSnapshotStore : ISnapshotStore
    {
        private readonly IMongoDatabase database;

        public MongoSnapshotStore(string connectionString)
        {
            var configuration = BuildMongoConfiguration(connectionString);
            var mongo = new Mongo(configuration);
            mongo.Connect();

            database = mongo.GetDatabase("snapshotstore");
        }

        private static MongoConfiguration BuildMongoConfiguration(string connectionString)
        {
            var configurationBuilder = new MongoConfigurationBuilder();
            configurationBuilder.ConnectionString(connectionString);
            configurationBuilder.Mapping(mapping => mapping.DefaultProfile(profile => profile.SubClassesAre(type => type.IsSubclassOf(typeof(ISnapshot)))));

            return configurationBuilder.BuildConfiguration();
        }

        public TSnapshot GetSnapshot<TSnapshot>(Guid aggregateRootId) where TSnapshot : class, ISnapshot
        {
            var snapshotsCollection = database.GetCollection<TSnapshot>("snapshots").Linq();
            return (from snapshot in snapshotsCollection
                    where snapshot.AggregateRootId == aggregateRootId
                    select snapshot).SingleOrDefault();
        }

        public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : class, ISnapshot
        {
            var snapshotsCollection = database.GetCollection<TSnapshot>("snapshots");
            snapshotsCollection.Save(snapshot);
        }
    }
}