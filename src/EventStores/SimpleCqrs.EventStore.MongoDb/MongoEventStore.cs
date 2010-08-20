using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB;
using MongoDB.Configuration;
using MongoDB.Configuration.Builders;
using SimpleCqrs.Core;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore.MongoDb
{
    public class MongoEventStore : IEventStore
    {
        private static readonly MethodInfo MapMethod = typeof(MappingStoreBuilder).GetMethod("Map", Type.EmptyTypes);
        private readonly IMongoDatabase database;

        public MongoEventStore(string connectionString, IDomainEventTypeCatalog domainEventTypeCatalog)
        {
            var configuration = BuildMongoConfiguration(domainEventTypeCatalog, connectionString);
            var mongo = new Mongo(configuration);
            mongo.Connect();

            database = mongo.GetDatabase("eventstore");
        }

        private static MongoConfiguration BuildMongoConfiguration(IDomainEventTypeCatalog domainEventTypeCatalog, string connectionString)
        {
            var configurationBuilder = new MongoConfigurationBuilder();
            configurationBuilder.ConnectionString(connectionString);
            configurationBuilder.Mapping(mapping =>
                                             {
                                                 mapping.DefaultProfile(profile => profile.SubClassesAre(type => type.IsSubclassOf(typeof(DomainEvent))));
                                                 domainEventTypeCatalog
                                                     .GetAllEventTypes()
                                                     .ForEach(type => MapEventType(type, mapping));
                                             });

            return configurationBuilder.BuildConfiguration();
        }

        public IEnumerable<DomainEvent> GetAggregateRootEvents(Guid aggregateRootId)
        {
            var eventsCollection = database.GetCollection<DomainEvent>("events").Linq();
            return (from domainEvent in eventsCollection
                   where domainEvent.AggregateRootId == aggregateRootId
                   select domainEvent).ToList();
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var eventsCollection = database.GetCollection<DomainEvent>("events");
            eventsCollection.Insert(domainEvents);
        }

        private static void MapEventType(Type type, MappingStoreBuilder mapping)
        {
            MapMethod.MakeGenericMethod(type)
                .Invoke(mapping, new object[] {});
        }
    }
}