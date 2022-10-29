using System.Collections.ObjectModel;
using System.Reflection;
using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly Queue<DomainEvent> _uncommittedEvents = new Queue<DomainEvent>();

        public Guid Id { get; protected internal set; }
        public int LastEventSequence { get; protected internal set; }

        public ReadOnlyCollection<DomainEvent> UncommittedEvents =>
            new ReadOnlyCollection<DomainEvent>(_uncommittedEvents.ToList());

        public void LoadFromHistoricalEvents(params DomainEvent[] domainEvents)
        {
            if (domainEvents.Length == 0)
            {
                return;
            }

            var domainEventList = domainEvents.OrderBy(domainEvent => domainEvent.Sequence).ToList();
            LastEventSequence = domainEventList.Last().Sequence;

            domainEventList.ForEach(ApplyEventToInternalState);
        }

        public void Apply(DomainEvent domainEvent)
        {
            domainEvent.Sequence = ++LastEventSequence;
            ApplyEventToInternalState(domainEvent);
            domainEvent.AggregateRootId = Id;
            domainEvent.EventDate = DateTime.UtcNow;

            EventModifier.Modify(domainEvent);

            _uncommittedEvents.Enqueue(domainEvent);
        }

        public void CommitEvents()
        {
            _uncommittedEvents.Clear();
            _entities.ForEach(entity => entity.CommitEvents());
        }

        public void RegisterEntity(Entity entity)
        {
            entity.AggregateRoot = this;
            _entities.Add(entity);
        }

        private void ApplyEventToInternalState(DomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var domainEventTypeName = domainEventType.Name;
            var aggregateRootType = GetType();

            var eventHandlerMethodName = GetEventHandlerMethodName(domainEventTypeName);
            var methodInfo = aggregateRootType.GetMethod(eventHandlerMethodName,
                BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic, null, new[] { domainEventType }, null);

            if (methodInfo != null && EventHandlerMethodInfoHasCorrectParameter(methodInfo, domainEventType))
            {
                methodInfo.Invoke(this, new[] { domainEvent });
            }

            ApplyEventToEntities(domainEvent);
        }

        private void ApplyEventToEntities(DomainEvent domainEvent)
        {
            if (!(domainEvent is EntityDomainEvent entityDomainEvent))
            {
                return;
            }

            var list = _entities.Where(entity => entity.Id == entityDomainEvent.EntityId).ToList();
            list.ForEach(entity => entity.ApplyHistoricalEvents(entityDomainEvent));
        }

        private static string GetEventHandlerMethodName(string domainEventTypeName)
        {
            var eventIndex = domainEventTypeName.LastIndexOf("Event");
            return "On" + domainEventTypeName.Remove(eventIndex, 5);
        }

        private static bool EventHandlerMethodInfoHasCorrectParameter(MethodBase eventHandlerMethodInfo,
            Type domainEventType)
        {
            var parameters = eventHandlerMethodInfo.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == domainEventType;
        }
    }
}