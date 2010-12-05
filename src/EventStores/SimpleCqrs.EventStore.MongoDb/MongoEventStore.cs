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
        private readonly IMongoDatabase database;

        public MongoEventStore(string connectionString, ITypeCatalog typeCatalog)
        {
            var connectionStringBuilder = new MongoConnectionStringBuilder(connectionString);
            var configuration = BuildMongoConfiguration(typeCatalog, connectionString);
            var mongo = new Mongo(configuration);
            mongo.Connect();

            database = mongo.GetDatabase(connectionStringBuilder.Database);
        }

        private static MongoConfiguration BuildMongoConfiguration(ITypeCatalog domainEventTypeCatalog, string connectionString)
        {
            var configurationBuilder = new MongoConfigurationBuilder();
            configurationBuilder.ConnectionString(connectionString);
            configurationBuilder.Mapping(mapping =>
                                             {
                                                 mapping.DefaultProfile(profile => profile.SubClassesAre(type => type.IsSubclassOf(typeof(DomainEvent))));
                                                 domainEventTypeCatalog
                                                     .GetDerivedTypes(typeof(DomainEvent))
                                                     .ForEach(type => MapEventType(type, mapping));
                                             });

            return configurationBuilder.BuildConfiguration();
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var eventsCollection = database.GetCollection<DomainEvent>("events").Linq();
            return (from domainEvent in eventsCollection
                    where domainEvent.AggregateRootId == aggregateRootId
                    where domainEvent.Sequence > startSequence
                    select domainEvent).ToList();
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var eventsCollection = database.GetCollection<DomainEvent>("events");
            eventsCollection.Insert(domainEvents);
        }

        public IEnumerable<DomainEvent> GetEventsOfTheseTypes(IEnumerable<Type> domainEventTypes)
        {
            throw new NotImplementedException();
        }

        private static void MapEventType(Type type, MappingStoreBuilder mapping)
        {
            MapMethod.MakeGenericMethod(type)
                .Invoke(mapping, new object[] {});
        }
    }
}