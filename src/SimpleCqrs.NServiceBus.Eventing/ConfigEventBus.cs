using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Eventing.Config;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public class ConfigEventBus : Configure
    {
        private readonly IDictionary<Type, string> domainEventTypeToDestinationLookup = new Dictionary<Type, string>();

        public IDictionary<Type, string> DomainEventTypeToDestinationLookup
        {
            get { return domainEventTypeToDestinationLookup; }
        }

        public void Configure(Configure config)
        {
            Configurer = config.Configurer;
            Builder = config.Builder;

            var domainEventBusConfig = GetConfigSection<DomainEventBusConfig>();
            var domainEventTypes = TypesToScan
                .Where(type => typeof(DomainEvent).IsAssignableFrom(type))
                .ToList();

            foreach (DomainEventEndpointMapping mapping in domainEventBusConfig.DomainEventEndpointMappings)
            {
                foreach (var domainEventType in domainEventTypes)
                {
                    if (DomainEventsIsTypeNameOrAssemblieNameForDomainEventType(domainEventType, mapping.DomainEvents))
                        domainEventTypeToDestinationLookup.Add(domainEventType, mapping.Endpoint);
                }
            }

            var eventHandlerTypes = GetDomainEventHandlerTypes();
            Configurer.RegisterSingleton<IEventBus>(new LocalEventBus(eventHandlerTypes, new DomainEventHandlerFactory()));
        }

        private static bool DomainEventsIsTypeNameOrAssemblieNameForDomainEventType(Type domainEventType, string domainEvents)
        {
            return domainEventType.AssemblyQualifiedName == domainEvents || domainEventType.Assembly.GetName().Name == domainEvents;
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