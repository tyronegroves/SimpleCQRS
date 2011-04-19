using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Domain
{
    public abstract class Entity : IHaveATestMode
    {
        private readonly Queue<EntityDomainEvent> uncommittedEvents = new Queue<EntityDomainEvent>();

        public AggregateRoot AggregateRoot { get; set; }
        public Guid Id { get; protected set; }
        bool IHaveATestMode.IsInTestMode { get; set; }

        public ReadOnlyCollection<EntityDomainEvent> UncommittedEvents
        {
            get { return new ReadOnlyCollection<EntityDomainEvent>(uncommittedEvents.ToList()); }
        }

        public void Apply(EntityDomainEvent domainEvent)
        {
            domainEvent.EntityId = Id;

            if(!((IHaveATestMode)this).IsInTestMode)
                AggregateRoot.Apply(domainEvent);

            uncommittedEvents.Enqueue(domainEvent);
            ApplyEventToInternalState(domainEvent);
        }

        public void ApplyHistoricalEvents(params EntityDomainEvent[] entityDomainEvents)
        {
            foreach(var entityDomainEvent in entityDomainEvents)
            {
                ApplyEventToInternalState(entityDomainEvent);    
            }
        }

        public void CommitEvents()
        {
            uncommittedEvents.Clear();
        }

        private void ApplyEventToInternalState(EntityDomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var domainEventTypeName = domainEventType.Name;
            var entityType = GetType();

            var methodInfo = entityType.GetMethod(GetEventHandlerMethodName(domainEventTypeName),
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