using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SimpleCqrs.Events;

namespace SimpleCqrs.Domain
{
    public abstract class AggregateRoot
    {
        private readonly Queue<DomainEvent> uncommittedEvents = new Queue<DomainEvent>();
        private int currentSequence;

        public Guid Id { get; protected set; }

        public ReadOnlyCollection<DomainEvent> UncommittedEvents
        {
            get { return new ReadOnlyCollection<DomainEvent>(uncommittedEvents.ToList()); }
        }

        public void ApplyEvents(params DomainEvent[] domainEvents)
        {
            domainEvents = domainEvents.OrderBy(domainEvent => domainEvent.Sequence).ToArray();
            currentSequence = domainEvents.Last().Sequence;

            foreach (var domainEvent in domainEvents)
            {
                var domainEventType = domainEvent.GetType();
                var domainEventTypeName = domainEventType.Name;
                var aggregateRootType = GetType();

                var methodInfos = aggregateRootType
                    .FindMembers(MemberTypes.Method,
                                 BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                 Type.FilterNameIgnoreCase,
                                 "On" + domainEventTypeName)
                    .Cast<MethodInfo>();

                foreach (var methodInfo in methodInfos)
                {
                    if (!EventHandlerMethodInfoHasCorrectParameter(methodInfo, domainEventType)) continue;

                    methodInfo.Invoke(this, new[] {domainEvent});
                    break;
                }
            }
        }

        public void CommitEvents()
        {
            uncommittedEvents.Clear();
        }

        protected void PublishEvent(DomainEvent domainEvent)
        {
            domainEvent.Sequence = ++currentSequence;
            ApplyEvents(domainEvent);
            uncommittedEvents.Enqueue(domainEvent);
        }

        private static bool EventHandlerMethodInfoHasCorrectParameter(MethodInfo eventHandlerMethodInfo, Type domainEventType)
        {
            var parameters = eventHandlerMethodInfo.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == domainEventType;
        }
    }
}