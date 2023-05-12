using EventSourcingCQRS.Eventing;
using EventSourcingCQRS.EventStore.MongoDb.RetryPolicies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly.Registry;
using Polly.Wrap;

namespace EventSourcingCQRS.EventStore.MongoDb
{
    public class MongoEventStore : IEventStore
    {
        private const string EventsCollection = "Events";
        private readonly string _className;
        private readonly ILogger<MongoEventStore> _logger;
        private readonly IMongoCollection<DomainEvent> _mongoCollection;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly AsyncPolicyWrap _asyncRetryPolicy;

        public MongoEventStore(
            ILogger<MongoEventStore> logger,
            IMongoClient mongoClient,
            IMongoEventStoreConfiguration mongoEventStoreConfiguration,
            IConfiguration configuration,
            IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            _className = GetType().FullName;
            _logger = logger;
            var databaseName = configuration.GetSection("Databases:EventStore").Value;
            mongoEventStoreConfiguration.Configure();
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
            _mongoCollection = _mongoDatabase.GetCollection<DomainEvent>(EventsCollection);
            _asyncRetryPolicy = policyRegistry.Get<AsyncPolicyWrap>(EventStoreRetryPolicy.RetryPolicyName);
        }

        public async Task<IEnumerable<Guid>> GetDistinctAggregateRootsId()
        {
            var filter = new BsonDocument();
            var documents = await _mongoCollection.DistinctAsync<Guid>("AggregateRootId", filter);
            return documents.ToList();
        }

        public async Task<IEnumerable<DomainEvent>> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var filter = Builders<DomainEvent>.Filter.Eq("AggregateRootId", aggregateRootId);
            filter &= Builders<DomainEvent>.Filter.Gt("Sequence", startSequence);

            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoCollection.Find(filter).ToListAsync();
            });
            return documents;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsUpToSequence(Guid aggregateRootId, int sequence)
        {
            var filter = Builders<DomainEvent>.Filter.Eq("AggregateRootId", aggregateRootId);
            filter &= Builders<DomainEvent>.Filter.Lte("Sequence", sequence);
            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoCollection.Find(filter).ToListAsync();
            });
            return documents;
        }

        public async Task Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var enumerable = domainEvents.ToList();
            _logger.LogInformation("{ClassName} Inserting {NumberOfEvents} Events ", _className, enumerable.Count());
            foreach (var e in enumerable)
            {
                _logger.LogInformation("{ClassName} Inserting {@Event}", _className, e);
            }

            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                await _mongoCollection.InsertManyAsync(enumerable);
            });
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            var filter = Builders<DomainEvent>.Filter.In("_t", domainEventTypes.Select(t => t.Name).ToArray());
            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoDatabase.GetCollection<DomainEvent>(EventsCollection).Find(filter).ToListAsync();
            });
            return documents;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes,
            Guid aggregateRootId)
        {
            var filter = Builders<DomainEvent>.Filter.Eq("AggregateRootId", aggregateRootId);
            filter &= Builders<DomainEvent>.Filter.In("_t", domainEventTypes.Select(t => t.Name).ToArray());
            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoDatabase.GetCollection<DomainEvent>(EventsCollection).Find(filter).ToListAsync();
            });
            return documents;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByEventTypesUpToSequence(IEnumerable<Type> domainEventTypes, Guid aggregateRootId, int sequence)
        {
            var eventsByType = await GetEventsByEventTypes(domainEventTypes, aggregateRootId);
            return eventsByType.TakeWhile(eventType => eventType.Sequence <= sequence).ToList();
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes,
            DateTime startDate, DateTime endDate)
        {
            var filter = Builders<DomainEvent>.Filter.Gt("EventDate", startDate);
            filter &= Builders<DomainEvent>.Filter.Lt("EventDate", endDate);
            filter &= Builders<DomainEvent>.Filter.In("_t", domainEventTypes.Select(t => t.Name).ToArray());
            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoDatabase.GetCollection<DomainEvent>(EventsCollection).Find(filter).ToListAsync();
            });
            return documents;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByCriteria(Dictionary<string, string> criteria)
        {
            var filter = Builders<DomainEvent>.Filter.Empty;

            foreach (var (key, value) in criteria)
            {
                filter &= Builders<DomainEvent>.Filter.Eq(key, value);
            }
            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoDatabase.GetCollection<DomainEvent>(EventsCollection).Find(filter).ToListAsync();
            });
            return documents;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByCriteria(List<EventQueryCriteria> criteria)
        {
            var filter = Builders<DomainEvent>.Filter.Empty;

            foreach (var item in criteria)
            {

                //var itemType = item.GetType();
                var itemType = typeof(EventQueryCriteria<>);
                var genericTypeArguments = item.GetType().GenericTypeArguments;
                if (genericTypeArguments.Length > 1)
                {
                    throw new Exception("Multiple generic type arguments are not supported.");
                }

                var genericTypeArgument = genericTypeArguments[0];
                var itemGeneric = itemType.MakeGenericType(genericTypeArguments);
                var instance = Activator.CreateInstance(itemGeneric);
                instance = item;

                var value = ((dynamic)Convert.ChangeType(item, itemGeneric)).Value;

                filter &= Builders<DomainEvent>.Filter.Eq(item.Name, value);
            }
            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoDatabase.GetCollection<DomainEvent>(EventsCollection).Find(filter).ToListAsync();
            });
            return documents;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsBySelector(BsonDocument selector)
        {
            List<DomainEvent> documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoDatabase.GetCollection<DomainEvent>(EventsCollection).Find(selector).ToListAsync();
            });
            return documents;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsBySelector(BsonDocument selector, int skip, int limit,
            CancellationToken cancellationToken)
        {
            var documents = new List<DomainEvent>();
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                await _mongoDatabase.GetCollection<DomainEvent>(EventsCollection)
                    .Find(selector)
                    .Skip(skip)
                    .Limit(limit)
                    .ForEachAsync(document => { documents.Add(document); }, cancellationToken);
            });
            return documents;
        }
    }
}