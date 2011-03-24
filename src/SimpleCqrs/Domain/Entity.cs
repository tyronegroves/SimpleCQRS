using System;
using System.Reflection;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Domain
{
    public abstract class Entity : IDomainEventProducer
    {
        public AggregateRoot Parent { get; set; }
        public Guid Id { get; protected set; }

        public void Apply(EntityDomainEvent domainEvent)
        {
            domainEvent.EntityId = Id;
            Parent.Apply(domainEvent);
            ApplyEventToInternalState(domainEvent);
        }

        public void ApplyHistoricalEvent(EntityDomainEvent domainEvent)
        {
            ApplyEventToInternalState(domainEvent);
        }

        private void ApplyEventToInternalState(EntityDomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var domainEventTypeName = domainEventType.Name;
            var aggregateRootType = GetType();

            var methodInfo = aggregateRootType.GetMethod(GetEventHandlerMethodName(domainEventTypeName),
                                                         BindingFlags.Instance | BindingFlags.Public |
                                                         BindingFlags.NonPublic, null, new[] {domainEventType}, null);

            if(methodInfo == null || !EventHandlerMethodInfoHasCorrectParameter(methodInfo, domainEventType)) return;

            methodInfo.Invoke(this, new[] {domainEvent});
        }

        private static string GetEventHandlerMethodName(string domainEventTypeName)
        {
            var eventIndex = domainEventTypeName.LastIndexOf("Event");
            return "On" + domainEventTypeName.Remove(eventIndex, 5);
        }

        private static bool EventHandlerMethodInfoHasCorrectParameter(MethodInfo eventHandlerMethodInfo, Type domainEventType)
        {
            var parameters = eventHandlerMethodInfo.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == domainEventType;
        }
    }
}