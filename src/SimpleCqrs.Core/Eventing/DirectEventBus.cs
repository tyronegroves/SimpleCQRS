using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCqrs.Eventing
{
    public class DirectEventBus : IEventBus
    {
        private readonly IServiceLocator serviceLocator;
        private IDictionary<Type, EventHandlerInvoker> eventHandlerInvokers;

        public DirectEventBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
            BuildEventInvokers(typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>)));
        }

        public void PublishEvent(DomainEvent domainEvent)
        {
            var eventHandlerInvoker = eventHandlerInvokers[domainEvent.GetType()];
            eventHandlerInvoker.Publish(domainEvent);
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            domainEvents.ForEach(PublishEvent);
        }

        private void BuildEventInvokers(IEnumerable<Type> eventHandlerTypes)
        {
            eventHandlerInvokers = new Dictionary<Type, EventHandlerInvoker>();
            foreach (var eventHandlerType in eventHandlerTypes)
            {
                foreach (var domainEventType in GetDomainEventTypes(eventHandlerType))
                {
                    EventHandlerInvoker eventInvoker;
                    if (!eventHandlerInvokers.TryGetValue(domainEventType, out eventInvoker))
                        eventInvoker = new EventHandlerInvoker(serviceLocator, domainEventType);

                    eventInvoker.AddEventHandlerType(eventHandlerType);
                    eventHandlerInvokers[domainEventType] = eventInvoker;
                }
            }
        }

        private static IEnumerable<Type> GetDomainEventTypes(Type eventHandlerType)
        {
            return from interfaceType in eventHandlerType.GetInterfaces()
                   where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                   select interfaceType.GetGenericArguments()[0];
        }

        private class EventHandlerInvoker
        {
            private readonly IServiceLocator serviceLocator;
            private readonly Type domainEventType;
            private readonly List<Type> eventHandlerTypes;

            public EventHandlerInvoker(IServiceLocator serviceLocator, Type domainEventType)
            {
                this.serviceLocator = serviceLocator;
                this.domainEventType = domainEventType;
                eventHandlerTypes = new List<Type>();
            }

            public void AddEventHandlerType(Type eventHandlerType)
            {
                eventHandlerTypes.Add(eventHandlerType);
            }

            public void Publish(DomainEvent domainEvent)
            {
                var handleMethod = typeof(IHandleDomainEvents<>).MakeGenericType(domainEventType).GetMethod("Handle");
                foreach (var eventHandlerType in eventHandlerTypes)
                {
                    var eventHandler = serviceLocator.Resolve(eventHandlerType);
                    handleMethod.Invoke(eventHandler, new object[] { domainEvent });
                }
            }

        }
    }
}