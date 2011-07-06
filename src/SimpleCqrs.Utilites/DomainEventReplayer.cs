using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Utilites
{
    public class DomainEventReplayer
    {
        private readonly ISimpleCqrsRuntime runtime;

        public DomainEventReplayer(ISimpleCqrsRuntime runtime)
        {
            this.runtime = runtime;
        }

        public void ReplayEventsForHandlerType(Type handlerType)
        {
            ReplayEventsForHandlerType(handlerType, DateTime.MinValue, DateTime.MaxValue);
        }

        public void ReplayEventsForHandlerType(Type handlerType, DateTime startDate, DateTime endDate)
        {
            runtime.Start();

            var serviceLocator = runtime.ServiceLocator;
            var eventStore = serviceLocator.Resolve<IEventStore>();
            var domainEventTypes = GetDomainEventTypesHandledByHandler(handlerType);

            var domainEvents = eventStore.GetEventsByEventTypes(domainEventTypes, startDate, endDate);
            var eventBus = new LocalEventBus(new[] {handlerType}, new DomainEventHandlerFactory(serviceLocator));

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