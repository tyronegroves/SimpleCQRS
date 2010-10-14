using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public class ConfigEventBus : Configure
    {
        public void Configure(Configure config)
        {
            var configurer = config.Configurer;
            var eventHandlerTypes = GetDomainEventHandlerTypes();
            var eventBus = new LocalEventBus(eventHandlerTypes, new DomainEventHandlerFactory());

            configurer.RegisterSingleton<IHandleMessages<DomainEventMessage>>(new DomainEventMessageHandler(eventBus));
        }

        private static IEnumerable<Type> GetDomainEventHandlerTypes()
        {
            return (from derivedType in TypesToScan
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                    select derivedType).Distinct().ToArray();
        }
    }
}