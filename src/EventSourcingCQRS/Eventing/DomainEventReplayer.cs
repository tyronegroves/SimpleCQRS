namespace EventSourcingCQRS.Eventing
{
    public class DomainEventReplayer : IDomainEventReplayer
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public DomainEventReplayer(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public async Task ReplayEventsForHandlerType(Type handlerType, CancellationToken cancellationToken)
        {
            await ReplayEventsForHandlerType(handlerType, DateTime.MinValue, DateTime.MaxValue, cancellationToken);
        }

        public async Task  ReplayEventsForHandlerType(Type handlerType, Guid aggregateRootId, CancellationToken cancellationToken)
        {
            var domainEventTypes = GetDomainEventTypesHandledByHandler(handlerType);
            var domainEvents = await _eventStore.GetEventsByEventTypes(domainEventTypes, aggregateRootId);
            await _eventBus.PublishEvents(domainEvents, cancellationToken);
        }

        public async Task ReplayEventsForHandlerType(Type handlerType, Guid aggregateRootId, int lastSequenceNo,
            CancellationToken cancellationToken)
        {
            var domainEventTypes = GetDomainEventTypesHandledByHandler(handlerType);
            var domainEvents = await _eventStore.GetEventsByEventTypesUpToSequence(domainEventTypes, aggregateRootId, lastSequenceNo);
            await _eventBus.PublishEvents(domainEvents, cancellationToken);
        }

        public async Task ReplayEventsForHandlerType(Type handlerType, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var domainEventTypes = GetDomainEventTypesHandledByHandler(handlerType);
            var domainEvents = await _eventStore.GetEventsByEventTypes(domainEventTypes, startDate, endDate);
            await _eventBus.PublishEvents(domainEvents, cancellationToken);
        }

        private static IEnumerable<Type> GetDomainEventTypesHandledByHandler(Type handlerType)
        {

            var interfaces = handlerType.GetInterfaces();
            var zero = interfaces[0];
            var genericType = zero.IsGenericType;
            var t = zero.GetGenericTypeDefinition();
            var isSame = t == typeof(IHandleDomainEvents<>);



            return (from i in handlerType.GetInterfaces()
                    where i.IsGenericType
                    where i.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                    select i.GetGenericArguments()[0]).ToList();
        }
    }
}