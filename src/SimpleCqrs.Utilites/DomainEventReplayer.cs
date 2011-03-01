using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.MongoDb;

namespace SimpleCqrs.Utilites
{
    public class DomainEventReplayer
    {
        private readonly ISimpleCqrsRuntime runtime;

        public DomainEventReplayer(ISimpleCqrsRuntime runtime)
        {
            this.runtime = runtime;
        }

        public void ReplayEventsForHandlerType(Type handlerType, Document selector)
        {
            runtime.Start();

            var serviceLocator = runtime.ServiceLocator;
            var eventStore = (MongoEventStore)serviceLocator.Resolve<IEventStore>();
            var domainEventTypes = GetDomainEventTypesHandledByHandler(handlerType);
            selector.Add("_t", new Document{{"$in", domainEventTypes}});
            var domainEvents = eventStore.GetEventsBySelector(selector);
            var eventBus = new LocalEventBus(new []{handlerType}, new DomainEventHandlerFactory(serviceLocator));

            eventBus.PublishEvents(domainEvents);
        }

        private static IEnumerable<Type> GetDomainEventTypesHandledByHandler(Type handlerType)
        {
            return (from i in handlerType.GetInterfaces()
                   where i.IsGenericType
                   where i.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                   select i.GetGenericArguments()[0]).ToList();
        }
    }
}