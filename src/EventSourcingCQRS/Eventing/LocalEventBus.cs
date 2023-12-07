namespace EventSourcingCQRS.Eventing
{
    public class LocalEventBus : IEventBus
    {
        private readonly IDomainEventHandlerFactory _eventHandlerBuilder;
        private IDictionary<Type, EventHandlerInvoker> _eventHandlerInvokers;

        public LocalEventBus(IEnumerable<Type> eventHandlerTypes, IDomainEventHandlerFactory eventHandlerBuilder)
        {
            _eventHandlerBuilder = eventHandlerBuilder;
            BuildEventInvokers(eventHandlerTypes);
        }

        public Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var domainEventType = domainEvent.GetType();
            var invokers = (from entry in _eventHandlerInvokers
                            where entry.Key.IsAssignableFrom(domainEventType)
                            select entry.Value).ToList();

            invokers.ForEach(i => i.Publish(domainEvent));

            return Task.CompletedTask;
        }

        public async Task PublishEvents(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            foreach (var domainEvent in domainEvents)
            {
                await Publish(domainEvent, cancellationToken);
            }
        }

        private void BuildEventInvokers(IEnumerable<Type> eventHandlerTypes)
        {
            _eventHandlerInvokers = new Dictionary<Type, EventHandlerInvoker>();
            foreach (var eventHandlerType in eventHandlerTypes)
            {
                foreach (var domainEventType in GetDomainEventTypes(eventHandlerType))
                {
                    if (!_eventHandlerInvokers.TryGetValue(domainEventType, out var eventInvoker))
                    {
                        eventInvoker = new EventHandlerInvoker(_eventHandlerBuilder, domainEventType);
                    }

                    eventInvoker.AddEventHandlerType(eventHandlerType);
                    _eventHandlerInvokers[domainEventType] = eventInvoker;
                }
            }
        }

        private static IEnumerable<Type> GetDomainEventTypes(Type eventHandlerType)
        {

            return from interfaceType in eventHandlerType.GetInterfaces()
                   where interfaceType.IsGenericType &&
                         interfaceType.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                   select interfaceType.GetGenericArguments()[0];
        }

        private class EventHandlerInvoker
        {
            private readonly Type _domainEventType;
            private readonly IDomainEventHandlerFactory _eventHandlerFactory;
            private readonly List<Type> _eventHandlerTypes;

            public EventHandlerInvoker(IDomainEventHandlerFactory eventHandlerFactory, Type domainEventType)
            {
                _eventHandlerFactory = eventHandlerFactory;
                _domainEventType = domainEventType;
                _eventHandlerTypes = new List<Type>();
            }

            public void AddEventHandlerType(Type eventHandlerType)
            {
                _eventHandlerTypes.Add(eventHandlerType);
            }

            public void Publish(DomainEvent domainEvent)
            {
                var handleMethod = typeof(IHandleDomainEvents<>).MakeGenericType(_domainEventType).GetMethod("Handle");
                foreach (var eventHandlerType in _eventHandlerTypes)
                {
                    var eventHandler = _eventHandlerFactory.Create(eventHandlerType);
                    handleMethod.Invoke(eventHandler, new object[] { domainEvent });
                }
            }
        }
    }
}