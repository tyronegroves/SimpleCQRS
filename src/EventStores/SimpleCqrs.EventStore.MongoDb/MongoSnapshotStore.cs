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
        private readonly MongoCollection<Snapshot> _collection;

        public MongoSnapshotStore(string connectionString, ITypeCatalog snapshotTypeCatalog)
        {
            snapshotTypeCatalog.GetDerivedTypes(typeof(Snapshot)).ToList().
                ForEach(x => BsonClassMap.LookupClassMap(x));

            _collection = MongoServer.Create(connectionString).GetDatabase("snapshotstore").GetCollection<Snapshot>("snapshots");
        }

        public Snapshot GetSnapshot(Guid aggregateRootId)
        {
            return _collection.Find(Query.EQ("AggregateRoootId", aggregateRootId)).
                SetFields(Fields.Exclude("_id")).Single();
        }

        public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot
        {
            _collection.Save(snapshot);
        }
    }
}