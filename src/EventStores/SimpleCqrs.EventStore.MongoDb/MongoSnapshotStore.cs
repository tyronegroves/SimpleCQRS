using System;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.MongoDb
{
    public class MongoSnapshotStore : ISnapshotStore
    {
        private readonly MongoCollection<Snapshot> collection;

        public MongoSnapshotStore(string connectionString, ITypeCatalog snapshotTypeCatalog)
        {
            snapshotTypeCatalog.GetDerivedTypes(typeof(Snapshot)).ToList().
                ForEach(x => BsonClassMap.LookupClassMap(x));

            collection = MongoServer.Create(connectionString).GetDatabase("snapshotstore").GetCollection<Snapshot>("snapshots");
        }

        public Snapshot GetSnapshot(Guid aggregateRootId)
        {
            return collection.FindOne(Query.EQ("AggregateRoootId", aggregateRootId));
        }

        public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot
        {
            collection.Save(snapshot);
        }
    }
}