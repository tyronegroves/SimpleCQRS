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
            Configurer = config.Configurer;
            Builder = config.Builder;

            var eventHandlerTypes = GetDomainEventHandlerTypes();
            Configurer.RegisterSingleton<IEventBus>(new LocalEventBus(eventHandlerTypes, new DomainEventHandlerFactory()));
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