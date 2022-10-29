using System.Collections.ObjectModel;
using System.Reflection;
using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.Domain
{
    public abstract class Entity
    {
        private readonly Queue<EntityDomainEvent> _uncommittedEvents = new Queue<EntityDomainEvent>();

        public AggregateRoot AggregateRoot { get; set; }

        public Guid Id { get; protected set; }

        public ReadOnlyCollection<EntityDomainEvent> UncommittedEvents =>
            new ReadOnlyCollection<EntityDomainEvent>(_uncommittedEvents.ToList());

        public void Apply(EntityDomainEvent domainEvent)
        {
            domainEvent.EntityId = Id;

            AggregateRoot.Apply(domainEvent);

            _uncommittedEvents.Enqueue(domainEvent);
            ApplyEventToInternalState(domainEvent);
        }

        public void ApplyHistoricalEvents(params EntityDomainEvent[] entityDomainEvents)
        {
            foreach (var entityDomainEvent in entityDomainEvents)
            {
                ApplyEventToInternalState(entityDomainEvent);
            }
        }

        public void CommitEvents()
        {
            _uncommittedEvents.Clear();
        }

        private void ApplyEventToInternalState(EntityDomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var domainEventTypeName = domainEventType.Name;
            var entityType = GetType();

            var methodInfo = entityType.GetMethod(GetEventHandlerMethodName(domainEventTypeName),
                BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic, null, new[] { domainEventType }, null);

            if (methodInfo == null || !EventHandlerMethodInfoHasCorrectParameter(methodInfo, domainEventType))
            {
                return;
            }

            methodInfo.Invoke(this, new[] { domainEvent });
        }

        private static string GetEventHandlerMethodName(string domainEventTypeName)
        {
            var eventIndex = domainEventTypeName.LastIndexOf("Event");
            return "On" + domainEventTypeName.Remove(eventIndex, 5);
        }

        private static bool EventHandlerMethodInfoHasCorrectParameter(MethodInfo eventHandlerMethodInfo,
            Type domainEventType)
        {
            var parameters = eventHandlerMethodInfo.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == domainEventType;
        }
    }
}