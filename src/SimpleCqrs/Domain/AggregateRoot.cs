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
        private readonly List<Entity> entities = new List<Entity>(); 

        public Guid Id { get; protected internal set; }
        public int LastEventSequence { get; protected internal set; }

        public ReadOnlyCollection<DomainEvent> UncommittedEvents
        {
            get { return new ReadOnlyCollection<DomainEvent>(uncommittedEvents.ToList()); }
        }

        public void LoadFromHistoricalEvents(params DomainEvent[] domainEvents)
        {
            if(domainEvents.Length == 0) return;

            var domainEventList = domainEvents.OrderBy(domainEvent => domainEvent.Sequence).ToList();
            LastEventSequence = domainEventList.Last().Sequence;

            domainEventList.ForEach(ApplyEventToInternalState);
        }

        public void Apply(DomainEvent domainEvent)
        {
            domainEvent.Sequence = ++LastEventSequence;
            ApplyEventToInternalState(domainEvent);
            domainEvent.AggregateRootId = Id;
            domainEvent.EventDate = DateTime.Now;
            
            EventModifier.Modify(domainEvent);

            uncommittedEvents.Enqueue(domainEvent);
        }

        public void CommitEvents()
        {
            uncommittedEvents.Clear();
            entities.ForEach(entity => entity.CommitEvents());
        }

        public void RegisterEntity(Entity entity)
        {
            entity.AggregateRoot = this;
            entities.Add(entity);
        }

        private void ApplyEventToInternalState(DomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var domainEventTypeName = domainEventType.Name;
            var aggregateRootType = GetType();

        	var eventHandlerMethodName = GetEventHandlerMethodName(domainEventTypeName);
        	var methodInfo = aggregateRootType.GetMethod(eventHandlerMethodName,
                                                         BindingFlags.Instance | BindingFlags.Public |
                                                         BindingFlags.NonPublic, null, new[] {domainEventType}, null);

            if(methodInfo != null && EventHandlerMethodInfoHasCorrectParameter(methodInfo, domainEventType))
            {
                methodInfo.Invoke(this, new[] {domainEvent});
            }

            ApplyEventToEntities(domainEvent);
        }

        private void ApplyEventToEntities(DomainEvent domainEvent)
        {
            var entityDomainEvent = domainEvent as EntityDomainEvent;
            if (entityDomainEvent == null) return;

            var list = entities
                .Where(entity => entity.Id == entityDomainEvent.EntityId).ToList();
            list
                .ForEach(entity => entity.ApplyHistoricalEvents(entityDomainEvent));
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