using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Domain
{
    public abstract class AggregateRoot
    {
        private readonly Queue<DomainEvent> uncommittedEvents = new Queue<DomainEvent>();

        public Guid Id { get; protected set; }
        public int LastEventSequence { get; private set; }

        public ReadOnlyCollection<DomainEvent> UncommittedEvents
        {
            get { return new ReadOnlyCollection<DomainEvent>(uncommittedEvents.ToList()); }
        }

        public void LoadFromHistoricalEvents(params DomainEvent[] domainEvents)
        {
            if(domainEvents.Length == 0) return;

            domainEvents = domainEvents.OrderBy(domainEvent => domainEvent.Sequence).ToArray();
            LastEventSequence = domainEvents.Last().Sequence;

            foreach(var domainEvent in domainEvents)
                ApplyEventToInternalState(domainEvent);
        }

        public void Apply(DomainEvent domainEvent)
        {
            domainEvent.Sequence = ++LastEventSequence;
            ApplyEventToInternalState(domainEvent);
            domainEvent.AggregateRootId = Id;
            domainEvent.EventDate = DateTime.Now;
            uncommittedEvents.Enqueue(domainEvent);
        }

        public void CommitEvents()
        {
            uncommittedEvents.Clear();
        }

        private void ApplyEventToInternalState(DomainEvent domainEvent)
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