using Azure.Data.Tables;
using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.EventStore.AzureTableStorage
{
    public class AzureTableEventStore : IEventStore
    {
        private readonly TableClient _client;
        private readonly IDomainEventSerializer _eventSerializer;

        //BlobContainerClient blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", "sample-container");
        //blobContainerClient.CreateIfNotExists();

        public AzureTableEventStore(TableClient tableClient, IDomainEventSerializer eventDeserializer)
        {
            _client = tableClient ?? throw new ArgumentNullException(nameof(tableClient));
            _eventSerializer = eventDeserializer ?? throw new ArgumentNullException(nameof(eventDeserializer));
        }

        private static EventData SerializeEvents(DomainEvent @event, IDomainEventSerializer eventSerializer)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            if (eventSerializer is null)
                throw new ArgumentNullException(nameof(eventSerializer));

            var json = eventSerializer.Serialize(@event);
            //var data = Encoding.UTF8.GetBytes(json);
            var data = json;
            var eventType = @event.GetType();

            return new EventData()
            {
                PartitionKey = @event.AggregateRootId.ToString(),
                RowKey = @event.Sequence.ToString("00000000000"),
                AggregateVersion = @event.Sequence,
                EventType = eventType.AssemblyQualifiedName,
                Data = data
            };
        }

        private static DomainEvent DeserializeEvents(EventData @event, IDomainEventSerializer eventSerializer)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            if (eventSerializer is null)
                throw new ArgumentNullException(nameof(eventSerializer));

            var domainEvent = eventSerializer.Deserialize(typeof(DomainEvent), @event.Data);
            //var data = Encoding.UTF8.GetBytes(json);

            return domainEvent;
        }

        public async Task Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var cancellationToken = new CancellationToken();
            var newEvents = domainEvents.Select(evt =>
            {
                var eventData = SerializeEvents(evt, _eventSerializer);
                return new TableTransactionAction(TableTransactionActionType.Add, eventData);
            }).ToArray();
            await _client.SubmitTransactionAsync(newEvents, cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<IEnumerable<Guid>> GetDistinctAggregateRootsId()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DomainEvent>> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var filter = $"PartitionKey eq '{aggregateRootId.ToString()}'";
            var entities = _client.Query<EventData>(filter);

            return entities.Select(entity => DeserializeEvents(entity, _eventSerializer)).ToList();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsUpToSequence(Guid aggregateRootId, int sequence)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypesUpToSequence(IEnumerable<Type> domainEventTypes, Guid aggregateRootId, int sequence)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByCriteria(Dictionary<string, string> criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByCriteria(List<EventQueryCriteria> criteria)
        {
            throw new NotImplementedException();
        }
    }
}