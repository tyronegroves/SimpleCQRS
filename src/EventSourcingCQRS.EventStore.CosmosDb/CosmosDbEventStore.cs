using EventSourcingCQRS.Eventing;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventSourcingCQRS.EventStore.CosmosDb
{
    public class CosmosDbEventStore : IEventStore
    {
        private readonly CosmosClient _client;
        private readonly Database _database;
        private readonly Container _container;
        private readonly IDomainEventSerializer _eventSerializer;

        public CosmosDbEventStore(CosmosClient cosmosClient, IDomainEventSerializer eventSerializer, string databaseName, string containerName)
        {
            _client = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _database = _client.GetDatabase(databaseName);
            _container = _database.GetContainer(containerName);
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
        }

        public CosmosDbEventStore(CosmosClient cosmosClient, IDomainEventSerializer eventSerializer, Database database, Container container)
        {
            _client = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _database = database;
            _container = container;
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
        }

        private  CosmosDbEventData SerializeEvents(DomainEvent @event)//, IDomainEventSerializer eventSerializer)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            //if (eventSerializer is null)
            //    throw new ArgumentNullException(nameof(eventSerializer));

            var json = _eventSerializer.Serialize(@event);
            //var data = Encoding.UTF8.GetBytes(json);
            var eventType = @event.GetType();

            return new CosmosDbEventData()
            {
                //PartitionKey = @event.AggregateRootId.ToString(),
                //RowKey = @event.Sequence.ToString("00000000000"),
                //AggregateVersion = @event.Sequence,
                Id = @event.Sequence.ToString("00000000000"),
                EventType = eventType.AssemblyQualifiedName,
                //EventData = json
                EventData = JObject.Parse(json)
            };
        }

        private  DomainEvent DeserializeEvents(CosmosDbEventData cosmosDbEvent)//, IDomainEventSerializer eventSerializer)
        {
            if (cosmosDbEvent is null)
                throw new ArgumentNullException(nameof(cosmosDbEvent));

            //if (eventSerializer is null)
            //    throw new ArgumentNullException(nameof(eventSerializer));

            //var domainEvent = eventSerializer.Deserialize(typeof(DomainEvent), @cosmosDbEvent.EventData);
            var domainEvent = _eventSerializer.Deserialize(typeof(DomainEvent), cosmosDbEvent.EventData);

            return domainEvent;
        }

        public async Task Insert(IEnumerable<DomainEvent> domainEvents)
        {
            //TODO: change method to work in a transaction
            /*
             *             var items = new List<object>();
        foreach (var json in jsonDocuments)
        {
            var item = new { Id = Guid.NewGuid().ToString(), EventData = json };
            items.Add(item);
        }

        await _container.CreateTransactionalBatch(new PartitionKey(items[0].Id))
            .CreateItems(items)
            .ExecuteAsync();
             */
            foreach (var domainEvent in domainEvents)
            {
                var json = SerializeEvents(domainEvent);//, _eventSerializer);
                //var item = new { Id = Guid.NewGuid().ToString(), Data = json };
                await _container.CreateItemAsync(json);
            }
        }

        public async Task<IEnumerable<DomainEvent>> GetEvents(Guid aggregateRootId, int startSequence)
        {

            var events = new List<DomainEvent>();
            var query = new QueryDefinition("SELECT * FROM c WHERE c.eventData.AggregateRootId = @id")
                .WithParameter("@id", aggregateRootId.ToString());
            var iterator = _container.GetItemQueryIterator<CosmosDbEventData>(query);

            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                var item = response.FirstOrDefault();

                if (item != null)
                {
                    var type = item.EventType;
                    var data = item.EventData;

                    try
                    {
                        events.Add(_eventSerializer.Deserialize(Type.GetType(type), data));
                    }
                    catch (ArgumentNullException ex)
                    {
                        throw new Exception(
                            $"Cannot find type '{type.Split(',')[0]}', yet the type is in the event store. Are you sure you haven't changed a class name or something arising from mental dullness?", ex.InnerException);
                    }



                    ////////var obj = JsonConvert.DeserializeObject<DomainEvent>(item.EventData.ToString());
                    ////////events.Add(obj);
                }
            }

            return events;
        }


        //public async Task<IEnumerable<DomainEvent>> GetEvents(Guid aggregateRootId, int startSequence)
        //{

        //    var events = new List<DomainEvent>();
        //    //var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
        //    var query = new QueryDefinition("SELECT * FROM c WHERE c.eventData.AggregateRootId = @id")

        //        .WithParameter("@id", aggregateRootId.ToString());
        //    var iterator = _container.GetItemQueryIterator<CosmosDbEventData>(query);

        //    if (iterator.HasMoreResults)
        //    {
        //        var response = await iterator.ReadNextAsync();
        //        var item = response.FirstOrDefault();

        //        if (item != null)
        //        {
        //            var obj = JsonConvert.DeserializeObject<DomainEvent>(item.EventData.ToString());
        //            events.Add(obj);
        //        }
        //    }

        //    return events;
        //}


        public Task<IEnumerable<Guid>> GetDistinctAggregateRootsId()
        {
            throw new NotImplementedException();
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