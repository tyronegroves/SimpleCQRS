using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Utilites
{
    public class DomainEventReplayer
    {
        private readonly IEventStore eventStore;
        private readonly IEventBus eventBus;

        public DomainEventReplayer(IEventStore eventStore, IEventBus eventBus)
        {
            this.eventStore = eventStore;
            this.eventBus = eventBus;
        }

        public void ReplayEventsForHandlerType(Type handlerType)
        {
            var domainEventTypes = GetDomainEventTypesHandledByHandler(handlerType);
            var domainEvents = eventStore.GetEventsByEventTypes(domainEventTypes);

            eventBus.PublishEvents(domainEvents);
        }

        private static IEnumerable<Type> GetDomainEventTypesHandledByHandler(Type handlerType)
        {
            return from i in handlerType.GetInterfaces()
                   where i.IsGenericType
                   where i.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                   select i.GetGenericArguments()[0];
        }
    }
}