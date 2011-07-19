using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.MongoDb
{
    public class MongoEventStore : IEventStore
    {
        private readonly MongoCollection<DomainEvent> _collection;

        public MongoEventStore(string connectionString, ITypeCatalog typeCatalog)
        {
            typeCatalog.GetDerivedTypes(typeof(DomainEvent)).ToList().
                ForEach(x => BsonClassMap.LookupClassMap(x));

            _collection = MongoServer.Create(connectionString).GetDatabase("events").GetCollection<DomainEvent>("events");
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            return _collection.Find(
                Query.And(
                    Query.EQ("AggregateRootId", aggregateRootId), 
                    Query.GT("Sequence", startSequence))).
                SetFields(Fields.Exclude("_id")).ToList();
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            _collection.InsertBatch(domainEvents);
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate)
        {
            return _collection.Find(
                Query.And(
                    Query.In("_t", domainEventTypes.Select(t => new BsonString(t.Name)).ToArray()), 
                    Query.GTE("EventDate", startDate),
                    Query.LTE("EventDate", endDate))).
                SetFields(Fields.Exclude("_id")).ToList();
        }

        public IEnumerable<DomainEvent> GetEventsBySelector(IMongoQuery selector, int skip, int limit)
        {
            return _collection.Find(selector).SetSkip(skip).SetLimit(limit);
        }

    }
}