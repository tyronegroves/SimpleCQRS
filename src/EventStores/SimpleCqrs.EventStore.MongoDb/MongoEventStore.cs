using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB;
using MongoDB.Configuration;
using MongoDB.Configuration.Builders;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.MongoDb
{
    public class MongoEventStore : IEventStore
    {
        private static readonly MethodInfo MapMethod = typeof(MappingStoreBuilder).GetMethod("Map", Type.EmptyTypes);
        private readonly MongoConfiguration configuration;
        private readonly string databaseName;

        public MongoEventStore(string connectionString, ITypeCatalog typeCatalog)
        {
            var connectionStringBuilder = new MongoConnectionStringBuilder(connectionString);
            databaseName = connectionStringBuilder.Database;
            configuration = BuildMongoConfiguration(typeCatalog, connectionString);
        }

        private static MongoConfiguration BuildMongoConfiguration(ITypeCatalog domainEventTypeCatalog, string connectionString)
        {
            var configurationBuilder = new MongoConfigurationBuilder();
            configurationBuilder.ConnectionString(connectionString);
            configurationBuilder.Mapping(mapping =>
            {
                mapping.DefaultProfile(profile => profile.SubClassesAre(t => t.IsSubclassOf(typeof(DomainEvent))));
                domainEventTypeCatalog
                    .GetDerivedTypes(typeof(DomainEvent))
                    .ToList()
                    .ForEach(type => MapEventType(type, mapping));
            });

            return configurationBuilder.BuildConfiguration();
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            using (var mongo = new Mongo(configuration))
            {
                mongo.Connect();

                var database = mongo.GetDatabase(databaseName);
                var eventsCollection = database.GetCollection<DomainEvent>("events").Linq();

                return (from domainEvent in eventsCollection
                        where domainEvent.AggregateRootId == aggregateRootId
                        where domainEvent.Sequence > startSequence
                        select domainEvent).ToList();
            }
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            using (var mongo = new Mongo(configuration))
            {
                mongo.Connect();

                var database = mongo.GetDatabase(databaseName);
                var eventsCollection = database.GetCollection<DomainEvent>("events");
                eventsCollection.Insert(domainEvents);
            }
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            using (var mongo = new Mongo(configuration))
            {
                mongo.Connect();

                var database = mongo.GetDatabase(databaseName);
                var document = new Document {{"_t", new Document {{"$in", domainEventTypes.Select(t => t.Name).ToArray()}}}};
                var cursor = database.GetCollection<DomainEvent>("events").Find(document);

                return cursor.Documents.ToList();
            }
        }

        public IEnumerable<DomainEvent> GetEventsBySelector(Document selector)
        {
            using (var mongo = new Mongo(configuration))
            {
                mongo.Connect();

                var database = mongo.GetDatabase(databaseName);
                var cursor = database.GetCollection<DomainEvent>("events").Find(selector);
                return cursor.Documents.ToList();
            }
        }

        private static void MapEventType(Type type, MappingStoreBuilder mapping)
        {
            MapMethod.MakeGenericMethod(type)
                .Invoke(mapping, new object[] { });
        }
    }
}